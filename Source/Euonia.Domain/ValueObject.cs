namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The value object contract.
/// </summary>
/// <typeparam name="TValueObject"></typeparam>
public class ValueObject<TValueObject> : IEquatable<TValueObject>
	where TValueObject : ValueObject<TValueObject>
{
	#region IEquatable and Override Equals operators

	/// <summary>
	/// Determine whether this value object is equals to other.
	/// </summary>
	/// <param name="other">The target object to compare.</param>
	/// <returns><c>true</c> if equals; otherwise <c>false</c></returns>
	public bool Equals(TValueObject other)
	{
		if (other == null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		//compare all public properties
		var publicProperties = GetType().GetProperties();

		if (publicProperties.Length > 0)
		{
			return publicProperties.All(property =>
			{
				var left = property.GetValue(this, null);
				var right = property.GetValue(other, null);

				if (left == null || right == null)
				{
					return false;
				}

				return left is TValueObject ? ReferenceEquals(left, right) : left.Equals(right);
			});
		}

		return true;
	}

	/// <inheritdoc/>
	/// <summary>
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		return obj is ValueObject<TValueObject> item && Equals((TValueObject)item);
	}

	/// <summary>
	/// Get hash code of this value object.
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
	{
		var hashCode = 31;
		var changeMultiplier = false;
		const int index = 1;

		//compare all public properties
		var publicProperties = GetType().GetProperties();

		if (publicProperties.Length == 0)
		{
			return hashCode;
		}

		foreach (var item in publicProperties)
		{
			var value = item.GetValue(this, null);

			if (value != null)
			{
				hashCode = hashCode * (changeMultiplier ? 59 : 114) + value.GetHashCode();

				changeMultiplier = !changeMultiplier;
			}
			else
			{
				hashCode ^= index * 13; //only for support {"a",null,null,"a"} <> {null,"a","a",null}
			}
		}

		return hashCode;
	}

	/// <summary>
	/// Implements the == operator.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator ==(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
	{
		return left?.Equals(right) ?? Equals(right, null);
	}

	/// <summary>
	/// Implements the != operator.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator !=(ValueObject<TValueObject> left, ValueObject<TValueObject> right)
	{
		return !(left == right);
	}

	#endregion
}
