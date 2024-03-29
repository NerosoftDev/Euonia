﻿using Nerosoft.Euonia.Caching.Internal;
using StackExchange.Redis;

namespace Nerosoft.Euonia.Caching.Redis;

/// <summary>
/// Cache handle implementation for Redis.
/// </summary>
/// <typeparam name="TValue">The type of the cache value.</typeparam>
public class RedisCacheHandle<TValue> : BaseCacheHandle<TValue>
{
	// ReSharper disable once StaticMemberInGenericType
	private static readonly TimeSpan _minimumExpirationTimeout = TimeSpan.FromMilliseconds(1);
	private const string BASE64_PREFIX = "base64\0";
	private const string HASH_FIELD_CREATED = "created";
	private const string HASH_FIELD_EXPIRATION_MODE = "expiration";
	private const string HASH_FIELD_EXPIRATION_TIMEOUT = "timeout";
	private const string HASH_FIELD_TYPE = "type";
	private const string HASH_FIELD_VALUE = "value";
	private const string HASH_FIELD_VERSION = "version";
	private const string HASH_FIELD_USES_DEFAULT_EXP = "defaultExpiration";

	private const string SCRIPT_ADD = $@"
if redis.call('HSETNX', KEYS[1], '{HASH_FIELD_VALUE}', ARGV[1]) == 1 then
    local result=redis.call('HMSET', KEYS[1], '{HASH_FIELD_TYPE}', ARGV[2], '{HASH_FIELD_EXPIRATION_MODE}', ARGV[3], '{HASH_FIELD_EXPIRATION_TIMEOUT}', ARGV[4], '{HASH_FIELD_CREATED}', ARGV[5], '{HASH_FIELD_VERSION}', 1, '{HASH_FIELD_USES_DEFAULT_EXP}', ARGV[6])
    if ARGV[3] > '1' and ARGV[4] ~= '0' then
        redis.call('PEXPIRE', KEYS[1], ARGV[4])
    else
        redis.call('PERSIST', KEYS[1])
    end
    return result
else
    return nil
end";

	private const string SCRIPT_PUT = $@"
local result=redis.call('HMSET', KEYS[1], '{HASH_FIELD_VALUE}', ARGV[1], '{HASH_FIELD_TYPE}', ARGV[2], '{HASH_FIELD_EXPIRATION_MODE}', ARGV[3], '{HASH_FIELD_EXPIRATION_TIMEOUT}', ARGV[4], '{HASH_FIELD_CREATED}', ARGV[5], '{HASH_FIELD_USES_DEFAULT_EXP}', ARGV[6])
redis.call('HINCRBY', KEYS[1], '{HASH_FIELD_VERSION}', 1)
if ARGV[3] > '1' and ARGV[4] ~= '0' then
    redis.call('PEXPIRE', KEYS[1], ARGV[4])
else
    redis.call('PERSIST', KEYS[1])
end
return result";

	// script should also update expire now. If sliding, update the sliding window
	private const string SCRIPT_UPDATE = $@"
if redis.call('HGET', KEYS[1], '{HASH_FIELD_VERSION}') == ARGV[2] then
    local result=redis.call('HSET', KEYS[1], '{HASH_FIELD_VALUE}', ARGV[1])
    redis.call('HINCRBY', KEYS[1], '{HASH_FIELD_VERSION}', 1)
    if ARGV[3] == '2' and ARGV[4] ~= '0' then
        redis.call('PEXPIRE', KEYS[1], ARGV[4])
    end
    return result;
else
    return nil
end";

	private const string SCRIPT_GET = $@"
local result = redis.call('HMGET', KEYS[1], '{HASH_FIELD_VALUE}', '{HASH_FIELD_EXPIRATION_MODE}', '{HASH_FIELD_EXPIRATION_TIMEOUT}', '{HASH_FIELD_CREATED}', '{HASH_FIELD_TYPE}', '{HASH_FIELD_VERSION}', '{HASH_FIELD_USES_DEFAULT_EXP}')
if (result[2] and result[2] == '2') then
    if (result[3] and result[3] ~= '' and result[3] ~= '0') then
        redis.call('PEXPIRE', KEYS[1], result[3])
    end
end
return result";

	private readonly Dictionary<ScriptType, LoadedLuaScript> _shaScripts = new();
	private readonly Dictionary<ScriptType, LuaScript> _luaScripts = new();
	private readonly CacheManagerConfiguration _managerConfiguration;
	private readonly RedisValueConverter _valueConverter;
	private readonly RedisConnectionManager _connection;
	private readonly bool _isLuaAllowed;
	private bool _canPreloadScripts;
	private readonly RedisConfiguration _redisConfiguration;

	// flag if scripts are initially loaded to the server
	private bool _scriptsLoaded;

	private readonly object _lockObject = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="RedisCacheHandle{TCacheValue}"/> class.
	/// </summary>
	/// <param name="managerConfiguration">The manager configuration.</param>
	/// <param name="configuration">The cache handle configuration.</param>
	/// <param name="connectionString"></param>
	public RedisCacheHandle(CacheManagerConfiguration managerConfiguration, CacheHandleConfiguration configuration, string connectionString)
		: base(managerConfiguration, configuration)
	{
		Check.EnsureNotNull(managerConfiguration, nameof(managerConfiguration));
		Check.EnsureNotNull(configuration, nameof(configuration));
		Check.EnsureNotNullOrWhiteSpace(connectionString, nameof(connectionString));

		_managerConfiguration = managerConfiguration;
		_valueConverter = new RedisValueConverter();
		_redisConfiguration = RedisConfigurations.GetConfiguration(configuration.Key, connectionString);
		_connection = new RedisConnectionManager(_redisConfiguration);
		_isLuaAllowed = _connection.Features.Scripting;

		// disable preloading right away if twemproxy mode, as this is not supported.
		_canPreloadScripts = !_redisConfiguration.TwemproxyEnabled;

		if (_redisConfiguration.KeyspaceNotificationsEnabled)
		{
			// notify-keyspace-events needs to be set to "Exe" at least! Otherwise we will not receive any events.
			// this must be configured per server and should probably not be done automagically as this needs admin rights!
			// Let's try to check at least if those settings are configured (the check also works only if useAdmin is set to true though).
			//try
			//{
			//    var configurations = _connection.GetConfiguration("notify-keyspace-events");
			//    foreach (var cfg in configurations)
			//    {
			//        if (!cfg.Value.Contains("E"))
			//        {
			//            Logger.LogWarn("Server {0} is missing configuration value 'E' in notify-keyspace-events to enable keyevents.", cfg.Key);
			//        }

			//        if (!(cfg.Value.Contains("A") ||
			//            (cfg.Value.Contains("x") && cfg.Value.Contains("e"))))
			//        {
			//            Logger.LogWarn("Server {0} is missing configuration value 'A' or 'x' and 'e' in notify-keyspace-events to enable keyevents for expired and evicted keys.", cfg.Key);
			//        }
			//    }
			//}
			//catch
			//{
			//    Logger.LogDebug("Could not read configuration from redis to validate notify-keyspace-events. Most likely useAdmin is not set to true.");
			//}

			SubscribeKeyspaceNotifications();
		}
	}

	/// <inheritdoc />
	public override bool IsDistributedCache => true;

	/// <summary>
	/// Gets the number of items the cache handle currently maintains.
	/// </summary>
	/// <value>The count.</value>
	/// <exception cref="InvalidOperationException">No active master found.</exception>
	public override int Count
	{
		get
		{
			if (_redisConfiguration.TwemproxyEnabled)
			{
				return 0;
			}

			var count = 0;
			foreach (var server in Servers.Where(p => !p.IsReplica && p.IsConnected))
			{
				count += (int)server.DatabaseSize(_redisConfiguration.Database);
			}

			// approx size, only size on the master..
			return count;
		}
	}

	/// <summary>
	/// Gets the servers.
	/// </summary>
	/// <value>The list of servers.</value>
	public IEnumerable<IServer> Servers => _connection.Servers;

	/// <summary>
	/// Gets the features the redis server supports.
	/// </summary>
	/// <value>The server features.</value>
	public RedisFeatures Features => _connection.Features;

	/// <summary>
	/// Gets a value indicating whether we can use the lua implementation instead of manual.
	/// This flag will be set automatically via feature detection based on the Redis server version
	/// or via <see cref="RedisConfiguration.StrictCompatibilityModeVersion"/> if set to a version which does not support lua scripting.
	/// </summary>
	public bool IsLuaAllowed => _isLuaAllowed;

	/// <summary>
	/// Clears this cache, removing all items in the base cache and all regions.
	/// </summary>
	public override void Clear()
	{
		try
		{
			foreach (var server in Servers.Where(p => !p.IsReplica))
			{
				Retry(() =>
				{
					if (server.IsConnected)
					{
						server.FlushDatabase(_redisConfiguration.Database);
					}
				});
			}
		}
		catch (NotSupportedException ex)
		{
			throw new NotSupportedException($"Clear is not available because '{ex.Message}'", ex);
		}
	}

	/// <summary>
	/// Clears the cache region, removing all items from the specified <paramref name="region"/> only.
	/// </summary>
	/// <param name="region">The cache region.</param>
	public override void ClearRegion(string region)
	{
		Retry(() =>
		{
			// we are storing all keys stored in the region in the hash for key=region
			var hashKeys = _connection.Database.HashKeys(region);

			if (hashKeys.Length > 0)
			{
				// lets remove all keys which where in the region
				// 01/32/16 changed to remove one by one because on clusters the keys could belong to multiple slots
				foreach (var key in hashKeys.Where(p => p.HasValue))
				{
					_connection.Database.KeyDelete(key.ToString(), CommandFlags.FireAndForget);
				}
			}

			// now delete the region
			_connection.Database.KeyDelete(region);
		});
	}

	/// <inheritdoc />
	public override bool Exists(string key)
	{
		var fullKey = GetKey(key);
		return Retry(() => _connection.Database.KeyExists(fullKey));
	}

	/// <inheritdoc />
	public override bool Exists(string key, string region)
	{
		Check.EnsureNotNullOrWhiteSpace(region, nameof(region));

		var fullKey = GetKey(key, region);
		return Retry(() => _connection.Database.KeyExists(fullKey));
	}

	/// <inheritdoc />
	public override CacheItemUpdateResult<TValue> Update(string key, Func<TValue, TValue> updateValue, int maxRetries)
		=> Update(key, null, updateValue, maxRetries);

	/// <inheritdoc />
	public override CacheItemUpdateResult<TValue> Update(string key, string region, Func<TValue, TValue> updateValue, int maxRetries)
	{
		if (!_isLuaAllowed)
		{
			return UpdateNoScript(key, region, updateValue, maxRetries);
		}

		var tries = 0;
		var fullKey = GetKey(key, region);

		return Retry(() =>
		{
			do
			{
				tries++;

				var item = GetCacheItemAndVersion(key, region, out int version);

				if (item == null)
				{
					return CacheItemUpdateResult.ForItemDidNotExist<TValue>();
				}

				ValidateExpirationTimeout(item);

				// run update
				var newValue = updateValue(item.Value);

				// added null check, throw explicit to me more consistent. Otherwise it would throw within the script exec
				if (newValue == null)
				{
					return CacheItemUpdateResult.ForFactoryReturnedNull<TValue>();
				}

				// resetting TTL on update, too
				var result = Eval(ScriptType.Update, fullKey, new[]
				{
					ToRedisValue(newValue),
					version,
					(int)item.ExpirationMode,
					(long)item.ExpirationTimeout.TotalMilliseconds,
				});

				if (result is { IsNull: false })
				{
					// optimizing not retrieving the item again after update (could have changed already, too)
					var newItem = item.WithValue(newValue);
					newItem.LastAccessedUtc = DateTime.UtcNow;

					return CacheItemUpdateResult.ForSuccess(newItem, tries > 1, tries);
				}
			}
			while (tries <= maxRetries);

			return CacheItemUpdateResult.ForTooManyRetries<TValue>(tries);
		});
	}

	private CacheItemUpdateResult<TValue> UpdateNoScript(string key, string region, Func<TValue, TValue> updateValue, int maxRetries)
	{
		bool committed;
		var tries = 0;
		var fullKey = GetKey(key, region);

		return Retry(() =>
		{
			do
			{
				tries++;

				var item = GetCacheItemInternal(key, region);

				if (item == null)
				{
					return CacheItemUpdateResult.ForItemDidNotExist<TValue>();
				}

				ValidateExpirationTimeout(item);

				var oldValue = ToRedisValue(item.Value);

				var tran = _connection.Database.CreateTransaction();
				tran.AddCondition(Condition.HashEqual(fullKey, HASH_FIELD_VALUE, oldValue));

				// run update
				var newValue = updateValue(item.Value);

				// added null check, throw explicit to me more consistent. Otherwise it would throw later
				if (newValue == null)
				{
					return CacheItemUpdateResult.ForFactoryReturnedNull<TValue>();
				}

				tran.HashSetAsync(fullKey, HASH_FIELD_VALUE, ToRedisValue(newValue));

				committed = tran.Execute();

				if (committed)
				{
					var newItem = item.WithValue(newValue);
					newItem.LastAccessedUtc = DateTime.UtcNow;

					if (newItem.ExpirationMode == CacheExpirationMode.Sliding && newItem.ExpirationTimeout != TimeSpan.Zero)
					{
						_connection.Database.KeyExpire(fullKey, newItem.ExpirationTimeout, CommandFlags.FireAndForget);
					}

					return CacheItemUpdateResult.ForSuccess(newItem, tries > 1, tries);
				}
			}
			while (committed == false && tries <= maxRetries);

			return CacheItemUpdateResult.ForTooManyRetries<TValue>(tries);
		});
	}

	/// <summary>
	/// Adds a value to the cache.
	/// <para>
	/// Add call is synced, so might be slower than put which is fire and forget but we want to
	/// return true|false if the operation was successfully or not. And always returning true
	/// could be misleading if the item already exists
	/// </para>
	/// </summary>
	/// <param name="item">The <c>CacheItem</c> to be added to the cache.</param>
	/// <returns>
	/// <c>true</c> if the key was not already added to the cache, <c>false</c> otherwise.
	/// </returns>
	protected override bool AddInternalPrepared(CacheItem<TValue> item) =>
		Retry(() => Set(item, When.NotExists, true));

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting
	/// unmanaged resources.
	/// </summary>
	/// <param name="disposeManaged">Indicator if managed resources should be released.</param>
	protected override void Dispose(bool disposeManaged)
	{
		base.Dispose(disposeManaged);
		if (disposeManaged)
		{
			// this.connection.RemoveConnection();
		}
	}

	/// <summary>
	/// Gets a <c>CacheItem</c> for the specified key.
	/// </summary>
	/// <param name="key">The key being used to identify the item within the cache.</param>
	/// <returns>The <c>CacheItem</c>.</returns>
	protected override CacheItem<TValue> GetCacheItemInternal(string key)
		=> GetCacheItemInternal(key, null);

	/// <summary>
	/// Gets a <c>CacheItem</c> for the specified key.
	/// </summary>
	/// <param name="key">The key being used to identify the item within the cache.</param>
	/// <param name="region">The cache region.</param>
	/// <returns>The <c>CacheItem</c>.</returns>
	protected override CacheItem<TValue> GetCacheItemInternal(string key, string region)
	{
		return GetCacheItemAndVersion(key, region, out _);
	}

	private CacheItem<TValue> GetCacheItemAndVersion(string key, string region, out int version)
	{
		version = -1;
		if (!_isLuaAllowed)
		{
			return GetCacheItemInternalNoScript(key, region);
		}

		var fullKey = GetKey(key, region);

		var result = Retry(() => Eval(ScriptType.Get, fullKey));
		if (result == null || result.IsNull)
		{
			// something went wrong. HMGET should return at least a null result for each requested field
			throw new InvalidOperationException("Error retrieving " + fullKey);
		}

		var values = (RedisValue[])result;

		// the first item stores the value
		var item = values![0];
		var expirationModeItem = values[1];
		var timeoutItem = values[2];
		var createdItem = values[3];
		var valueTypeItem = values[4];
		version = (int)values[5];
		var usesDefaultExpiration = values[6].HasValue
			? (bool)values[6] // if flag is set, all good...
			: !expirationModeItem.HasValue || !timeoutItem.HasValue; // otherwise fall back to use default expiration from config

		if (!item.HasValue || !valueTypeItem.HasValue /* partially removed? */
		                   || item.IsNullOrEmpty || item.IsNull)
		{
			return null;
		}

		var expirationMode = CacheExpirationMode.None;
		var expirationTimeout = default(TimeSpan);

		// checking if the expiration mode is set on the hash
		if (expirationModeItem.HasValue && timeoutItem.HasValue)
		{
			if (!timeoutItem.IsNullOrEmpty && !expirationModeItem.IsNullOrEmpty)
			{
				expirationMode = (CacheExpirationMode)(int)expirationModeItem;
				expirationTimeout = TimeSpan.FromMilliseconds((long)timeoutItem);
			}
			else
			{
			}
		}

		var value = FromRedisValue(item, valueTypeItem);

		var cacheItem =
			usesDefaultExpiration ? string.IsNullOrWhiteSpace(region) ? new CacheItem<TValue>(key, value) : new CacheItem<TValue>(key, region, value) :
			string.IsNullOrWhiteSpace(region) ? new CacheItem<TValue>(key, value, expirationMode, expirationTimeout) :
			new CacheItem<TValue>(key, region, value, expirationMode, expirationTimeout);

		if (createdItem.HasValue)
		{
			cacheItem = cacheItem.WithCreated(new DateTime((long)createdItem, DateTimeKind.Utc));
		}

		if (cacheItem.IsExpired)
		{
			TriggerCacheSpecificRemove(key, region, CacheItemRemovedReason.Expired, cacheItem.Value);

			return null;
		}

		return cacheItem;
	}

	/// <summary>
	/// Gets a <c>CacheItem</c> for the specified key without using lua script.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="region"></param>
	/// <returns></returns>
	protected virtual CacheItem<TValue> GetCacheItemInternalNoScript(string key, string region)
	{
		return Retry(() =>
		{
			var fullKey = GetKey(key, region);

			// getting both, the value and, if exists, the expiration mode. if that one is set
			// and it is sliding, we also retrieve the timeout later
			var values = _connection.Database.HashGet(
				fullKey,
				new RedisValue[]
				{
					HASH_FIELD_VALUE,
					HASH_FIELD_EXPIRATION_MODE,
					HASH_FIELD_EXPIRATION_TIMEOUT,
					HASH_FIELD_CREATED,
					HASH_FIELD_TYPE,
					HASH_FIELD_USES_DEFAULT_EXP
				});

			// the first item stores the value
			var item = values[0];
			var expirationModeItem = values[1];
			var timeoutItem = values[2];
			var createdItem = values[3];
			var valueTypeItem = values[4];
			var usesDefaultExpiration = values[5].HasValue
				? (bool)values[5] // if flag is set, all good...
				: !expirationModeItem.HasValue || !timeoutItem.HasValue; // otherwise fall back to use default expiration from config

			if (!item.HasValue || !valueTypeItem.HasValue /* partially removed? */
			                   || item.IsNullOrEmpty || item.IsNull)
			{
				return null;
			}

			var expirationMode = CacheExpirationMode.None;
			var expirationTimeout = default(TimeSpan);

			// checking if the expiration mode is set on the hash
			if (expirationModeItem.HasValue && timeoutItem.HasValue)
			{
				// adding sanity check for empty string results. Could happen in rare cases like #74
				if (!timeoutItem.IsNullOrEmpty && !expirationModeItem.IsNullOrEmpty)
				{
					expirationMode = (CacheExpirationMode)(int)expirationModeItem;
					expirationTimeout = TimeSpan.FromMilliseconds((long)timeoutItem);
				}
				else
				{
				}
			}

			var value = FromRedisValue(item, valueTypeItem);

			var cacheItem =
				usesDefaultExpiration ? string.IsNullOrWhiteSpace(region) ? new CacheItem<TValue>(key, value) : new CacheItem<TValue>(key, region, value) :
				string.IsNullOrWhiteSpace(region) ? new CacheItem<TValue>(key, value, expirationMode, expirationTimeout) :
				new CacheItem<TValue>(key, region, value, expirationMode, expirationTimeout);

			if (createdItem.HasValue)
			{
				cacheItem = cacheItem.WithCreated(new DateTime((long)createdItem, DateTimeKind.Utc));
			}

			if (cacheItem.IsExpired)
			{
				TriggerCacheSpecificRemove(key, region, CacheItemRemovedReason.Expired, cacheItem.Value);

				return null;
			}

			// update sliding
			if (expirationMode == CacheExpirationMode.Sliding && expirationTimeout != default)
			{
				_connection.Database.KeyExpire(fullKey, cacheItem.ExpirationTimeout, CommandFlags.FireAndForget);
			}

			return cacheItem;
		});
	}

	/// <summary>
	/// Puts the <paramref name="item"/> into the cache. If the item exists it will get updated
	/// with the new value. If the item doesn't exist, the item will be added to the cache.
	/// </summary>
	/// <param name="item">The <c>CacheItem</c> to be added to the cache.</param>
	protected override void PutInternalPrepared(CacheItem<TValue> item) =>
		Retry(() => Set(item, When.Always));

	/// <summary>
	/// Removes a value from the cache for the specified key.
	/// </summary>
	/// <param name="key">The key being used to identify the item within the cache.</param>
	/// <returns>
	/// <c>true</c> if the key was found and removed from the cache, <c>false</c> otherwise.
	/// </returns>
	protected override bool RemoveInternal(string key) => RemoveInternal(key, null);

	/// <summary>
	/// Removes a value from the cache for the specified key.
	/// </summary>
	/// <param name="key">The key being used to identify the item within the cache.</param>
	/// <param name="region">The cache region.</param>
	/// <returns>
	/// <c>true</c> if the key was found and removed from the cache, <c>false</c> otherwise.
	/// </returns>
	protected override bool RemoveInternal(string key, string region)
	{
		return Retry(() =>
		{
			var fullKey = GetKey(key, region);

			// clean up region
			if (!string.IsNullOrWhiteSpace(region))
			{
				_connection.Database.HashDelete(region, fullKey, CommandFlags.FireAndForget);
			}

			// remove key
			var result = _connection.Database.KeyDelete(fullKey);

			return result;
		});
	}

	private void SubscribeKeyspaceNotifications()
	{
		_connection.Subscriber.Subscribe(
			new RedisChannel($"__keyevent@{_redisConfiguration.Database}__:expired", RedisChannel.PatternMode.Auto),
			(_, key) =>
			{
				var tuple = ParseKey(key);
				// we cannot return the original value here because we don't have it
				TriggerCacheSpecificRemove(tuple.Item1, tuple.Item2, CacheItemRemovedReason.Expired, null);
			});

		_connection.Subscriber.Subscribe(
			new RedisChannel($"__keyevent@{_redisConfiguration.Database}__:evicted", RedisChannel.PatternMode.Auto),
			(_, key) =>
			{
				var tuple = ParseKey(key);

				// we cannot return the original value here because we don't have it
				TriggerCacheSpecificRemove(tuple.Item1, tuple.Item2, CacheItemRemovedReason.Evicted, null);
			});

		_connection.Subscriber.Subscribe(
			new RedisChannel($"__keyevent@{_redisConfiguration.Database}__:del", RedisChannel.PatternMode.Auto),
			(_, key) =>
			{
				var tuple = ParseKey(key);

				// we cannot return the original value here because we don't have it
				TriggerCacheSpecificRemove(tuple.Item1, tuple.Item2, CacheItemRemovedReason.ExternalDelete, null);
			});
	}

	private static Tuple<string, string> ParseKey(string value)
	{
		if (value == null)
		{
			return Tuple.Create<string, string>(null, null);
		}

		var sepIndex = value.IndexOf(':');
		var hasRegion = sepIndex > 0;
		var key = value;
		string region = null;

		if (hasRegion)
		{
			region = value[..sepIndex];
			key = value[(sepIndex + 1)..];

			if (region.StartsWith(BASE64_PREFIX))
			{
				region = region[BASE64_PREFIX.Length..];
				region = Encoding.UTF8.GetString(Convert.FromBase64String(region));
			}
		}

		if (key.StartsWith(BASE64_PREFIX))
		{
			key = key[BASE64_PREFIX.Length..];
			key = Encoding.UTF8.GetString(Convert.FromBase64String(key));
		}

		return Tuple.Create(key, region);
	}

	private static void ValidateExpirationTimeout(CacheItem<TValue> item)
	{
		if ((item.ExpirationMode == CacheExpirationMode.Absolute || item.ExpirationMode == CacheExpirationMode.Sliding) && item.ExpirationTimeout < _minimumExpirationTimeout)
		{
			throw new ArgumentException(Resources.IDS_TIMEOUT_LOWER_THAN_ONE_MILLISECOND_NOT_SUPPORTED, nameof(item.ExpirationTimeout));
		}
	}

	private string GetKey(string key, string region = null)
	{
		if (string.IsNullOrWhiteSpace(key))
		{
			throw new ArgumentNullException(nameof(key));
		}

		// for notifications, we have to get key and region back from the key stored in redis.
		// in case the key and or region itself contains the separator, there would be no way to do so...
		// So, only if that feature is enabled, we'll encode the key and/or region in that case
		// and the ParseKey method will respect that, too, and decodes the key and/or region.
		if (_redisConfiguration.KeyspaceNotificationsEnabled && key.Contains(":"))
		{
			key = BASE64_PREFIX + Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
		}

		var fullKey = key;

		if (!string.IsNullOrWhiteSpace(region))
		{
			if (_redisConfiguration.KeyspaceNotificationsEnabled && region.Contains(":"))
			{
				region = BASE64_PREFIX + Convert.ToBase64String(Encoding.UTF8.GetBytes(region));
			}

			fullKey = string.Concat(region, ":", key);
		}

		return fullKey;
	}

	private TValue FromRedisValue(RedisValue value, string valueType)
	{
		if (value.IsNull || value.IsNullOrEmpty || !value.HasValue)
		{
			return default;
		}

		if (_valueConverter is IRedisValueConverter<TValue> typedConverter)
		{
			return typedConverter.FromRedisValue(value, valueType);
		}

		return _valueConverter.FromRedisValue<TValue>(value, valueType);
	}

	private RedisValue ToRedisValue(TValue value)
	{
		if (_valueConverter is IRedisValueConverter<TValue> typedConverter)
		{
			return typedConverter.ToRedisValue(value);
		}

		return _valueConverter.ToRedisValue(value);
	}

	private T Retry<T>(Func<T> action) =>
		RetryHelper.Retry(action, _managerConfiguration.RetryTimeout, _managerConfiguration.MaxRetries);

	private void Retry(Action action)
		=> Retry(
			() =>
			{
				action();
				return true;
			});

	private bool Set(CacheItem<TValue> item, When when, bool sync = false)
	{
		if (!_isLuaAllowed)
		{
			return SetNoScript(item, when, sync);
		}

		var fullKey = GetKey(item.Key, item.Region);
		var value = ToRedisValue(item.Value);

		var flags = sync ? CommandFlags.None : CommandFlags.FireAndForget;

		ValidateExpirationTimeout(item);

		// ARGV [1]: value, [2]: type, [3]: expirationMode, [4]: expirationTimeout(millis), [5]: created(ticks)
		var parameters = new RedisValue[]
		{
			value,
			item.ValueType.AssemblyQualifiedName,
			(int)item.ExpirationMode,
			(long)item.ExpirationTimeout.TotalMilliseconds,
			item.CreatedUtc.Ticks,
			item.UsesExpirationDefaults
		};

		RedisResult result;
		if (when == When.NotExists)
		{
			result = Eval(ScriptType.Add, fullKey, parameters, flags);
		}
		else
		{
			result = Eval(ScriptType.Put, fullKey, parameters, flags);
		}

		if (result == null)
		{
			if (flags.HasFlag(CommandFlags.FireAndForget))
			{
				if (!string.IsNullOrWhiteSpace(item.Region))
				{
					// setting region lookup key if region is being used
					_connection.Database.HashSet(item.Region, fullKey, "regionKey", When.Always, CommandFlags.FireAndForget);
				}

				// put runs via fire and forget, so we don't get a result back
				return true;
			}

			// should never happen, something went wrong with the script
			throw new InvalidOperationException("Something went wrong adding an item, result must not be null.");
		}
		else
		{
			if (result.IsNull && when == When.NotExists)
			{
				return false;
			}

			var resultValue = (RedisValue)result;

			if (resultValue.HasValue && resultValue.ToString().Equals("OK", StringComparison.OrdinalIgnoreCase))
			{
				// Added successfully:
				if (!string.IsNullOrWhiteSpace(item.Region))
				{
					// setting region lookup key if region is being used
					// we cannot do that within the lua because the region could be on another cluster node!
					_connection.Database.HashSet(item.Region, fullKey, "regionKey", When.Always, CommandFlags.FireAndForget);
				}

				return true;
			}

			return false;
		}
	}

	private bool SetNoScript(CacheItem<TValue> item, When when, bool sync = false)
	{
		return Retry(() =>
		{
			var fullKey = GetKey(item.Key, item.Region);
			var value = ToRedisValue(item.Value);

			ValidateExpirationTimeout(item);

			var metaValues = new[]
			{
				new HashEntry(HASH_FIELD_TYPE, item.ValueType.AssemblyQualifiedName),
				new HashEntry(HASH_FIELD_EXPIRATION_MODE, (int)item.ExpirationMode),
				new HashEntry(HASH_FIELD_EXPIRATION_TIMEOUT, (long)item.ExpirationTimeout.TotalMilliseconds),
				new HashEntry(HASH_FIELD_CREATED, item.CreatedUtc.Ticks),
				new HashEntry(HASH_FIELD_USES_DEFAULT_EXP, item.UsesExpirationDefaults)
			};

			var flags = sync ? CommandFlags.None : CommandFlags.FireAndForget;

			var setResult = _connection.Database.HashSet(fullKey, HASH_FIELD_VALUE, value, when, flags);

			// setResult from fire and forget is alwys false, so we have to assume it works...
			setResult = flags == CommandFlags.FireAndForget || setResult;

			if (!setResult)
			{
				return false;
			}

			if (!string.IsNullOrWhiteSpace(item.Region))
			{
				// setting region lookup key if region is being used
				_connection.Database.HashSet(item.Region, fullKey, "regionKey", When.Always, CommandFlags.FireAndForget);
			}

			// set the additional fields in case sliding expiration should be used in this
			// case we have to store the expiration mode and timeout on the hash, too so
			// that we can extend the expiration period every time we do a get
			_connection.Database.HashSet(fullKey, metaValues, flags);

			if (item.ExpirationMode != CacheExpirationMode.None && item.ExpirationMode != CacheExpirationMode.Default)
			{
				_connection.Database.KeyExpire(fullKey, item.ExpirationTimeout, CommandFlags.FireAndForget);
			}
			else
			{
				// bugfix #9
				_connection.Database.KeyPersist(fullKey, CommandFlags.FireAndForget);
			}

			return true;
		});
	}

	private RedisResult Eval(ScriptType scriptType, RedisKey redisKey, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
	{
		if (!_scriptsLoaded)
		{
			lock (_lockObject)
			{
				if (!_scriptsLoaded)
				{
					LoadScripts();
					_scriptsLoaded = true;
				}
			}
		}

		LoadedLuaScript script = null;
		if (!_luaScripts.TryGetValue(scriptType, out LuaScript luaScript)
		    || (_canPreloadScripts && !_shaScripts.TryGetValue(scriptType, out script)))
		{
			_scriptsLoaded = false;
			throw new InvalidOperationException("Something is wrong with the Lua scripts. Seem to be not loaded.");
		}

		try
		{
			if (_canPreloadScripts && script != null)
			{
				return _connection.Database.ScriptEvaluate(script.Hash, new[] { redisKey }, values, flags);
			}
			else
			{
				return _connection.Database.ScriptEvaluate(luaScript.ExecutableScript, new[] { redisKey }, values, flags);
			}
		}
		catch (RedisServerException ex) when (ex.Message.StartsWith("NOSCRIPT", StringComparison.OrdinalIgnoreCase))
		{
			LoadScripts();

			// retry
			throw;
		}
	}

	private void LoadScripts()
	{
		lock (_lockObject)
		{
			var putLua = LuaScript.Prepare(SCRIPT_PUT);
			var addLua = LuaScript.Prepare(SCRIPT_ADD);
			var updateLua = LuaScript.Prepare(SCRIPT_UPDATE);
			var getLua = LuaScript.Prepare(SCRIPT_GET);
			_luaScripts.Clear();
			_luaScripts.Add(ScriptType.Add, addLua);
			_luaScripts.Add(ScriptType.Put, putLua);
			_luaScripts.Add(ScriptType.Update, updateLua);
			_luaScripts.Add(ScriptType.Get, getLua);

			// servers feature might be disabled
			if (_canPreloadScripts)
			{
				try
				{
					foreach (var server in Servers)
					{
						if (server.IsConnected)
						{
							_shaScripts[ScriptType.Put] = putLua.Load(server);
							_shaScripts[ScriptType.Add] = addLua.Load(server);
							_shaScripts[ScriptType.Update] = updateLua.Load(server);
							_shaScripts[ScriptType.Get] = getLua.Load(server);
						}
					}
				}
				catch (NotSupportedException)
				{
					_canPreloadScripts = false;
				}
			}
		}
	}
}