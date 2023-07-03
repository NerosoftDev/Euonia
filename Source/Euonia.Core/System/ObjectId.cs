
/// <summary>
/// The object identifier.
/// </summary>
public readonly struct ObjectId
{
    /// <summary>
    /// Create new <see cref="ObjectId"/> use <see cref="int"/> value.
    /// </summary>
    /// <param name="value"></param>
    public ObjectId(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> use <see cref="long"/> value.
    /// </summary>
    /// <param name="value"></param>
    public ObjectId(long value)
    {
        Value = value;
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> use <see cref="System.Guid"/> value.
    /// </summary>
    /// <param name="value"></param>
    public ObjectId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> use <see cref="string"/> value.
    /// </summary>
    /// <param name="value"></param>
    public ObjectId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the identifier actual value.
    /// </summary>
    public object Value { get; }

    public static bool operator ==(ObjectId id1, ObjectId id2) => EqualityComparer<object>.Default.Equals(id1.Value, id2.Value);

    public static bool operator !=(ObjectId id1, ObjectId id2) => !EqualityComparer<object>.Default.Equals(id1.Value, id2.Value);

    public static implicit operator ObjectId(long id)
    {
        return new ObjectId(id);
    }

    public static implicit operator long(ObjectId id)
    {
        return (long)id.Value;
    }

    public static implicit operator ObjectId(int id)
    {
        return new ObjectId(id);
    }

    public static implicit operator int(ObjectId id)
    {
        return (int)id.Value;
    }

    public static implicit operator ObjectId(string id)
    {
        return new ObjectId(id);
    }

    public static implicit operator string(ObjectId id)
    {
        return (string)id.Value;
    }

    public static implicit operator ObjectId(Guid id)
    {
        return new ObjectId(id);
    }

    public static implicit operator Guid(ObjectId id)
    {
        return (Guid)id.Value;
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> instance use snowflake id.
    /// </summary>
    /// <returns></returns>
    public static ObjectId Snowflake()
    {
        return new ObjectId(NewSnowflake());
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> instance use <see cref="System.Guid"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ObjectId Guid(GuidType type)
    {
        return new ObjectId(NewGuid(type));
    }

    /// <summary>
    /// Create new <see cref="ObjectId"/> instance use random string value.
    /// </summary>
    /// <returns></returns>
    public static ObjectId Random()
    {
        return new ObjectId(NewRandomId(DateTime.UtcNow.Ticks));
    }

    /// <summary>
    /// Generate new snowflake id.
    /// </summary>
    /// <returns></returns>
    public static long NewSnowflake()
    {
        return SnowflakeId.Instance.Next();
    }

    /// <summary>
    /// Generate new <see cref="System.Guid"/> use specifed <see cref="GuidType"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Guid NewGuid(GuidType type)
    {
        return GuidGenerator.Generate(type);
    }

    /// <summary>
    /// Generate an new random string id.
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public static string NewRandomId(long seed)
    {
        return RandomId.Generate(seed);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not ObjectId id)
        {
            return false;
        }

        return id.Value.Equals(Value);
    }
}

/// <summary>
/// The object identifier with value of type <see cref="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct ObjectId<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Create new <see cref="ObjectId{T}"/> instance.
    /// </summary>
    /// <param name="value"></param>
    public ObjectId(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the identifier actual value.
    /// </summary>
    public T Value { get; }

    public static bool operator ==(ObjectId<T> id1, ObjectId<T> id2) => EqualityComparer<T>.Default.Equals(id1.Value, id2.Value);

    public static bool operator !=(ObjectId<T> id1, ObjectId<T> id2) => !EqualityComparer<T>.Default.Equals(id1.Value, id2.Value);

    public static implicit operator ObjectId<T>(T id)
    {
        return new ObjectId<T>(id);
    }

    public static implicit operator T(ObjectId<T> id)
    {
        return id.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is ObjectId<T> id &&
               EqualityComparer<T>.Default.Equals(Value, id.Value);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}