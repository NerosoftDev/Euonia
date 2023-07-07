namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Enumerates the query operators.
/// </summary>
public enum QueryOperator
{
    /// <summary>
    /// Represents = value.
    /// </summary>
    Equal,

    /// <summary>
    /// Represents LIKE value%.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Represents LIKE %value.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Represents LIKE %value%.
    /// </summary>
    Contains,

    /// <summary>
    /// Represents NOT LIKE %value%.
    /// </summary>
    NotContains,

    /// <summary>
    /// Represents LIKE value.
    /// </summary>
    Like,

    /// <summary>
    /// Represents NOT LIKE value.
    /// </summary>
    NotLike,

    /// <summary>
    /// Represents IS value
    /// </summary>
    Is,

    /// <summary>
    /// 
    /// </summary>
    NotEqual,

    /// <summary>
    /// 
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// 
    /// </summary>
    LessThan,

    /// <summary>
    /// 
    /// </summary>
    LessThanOrEqual
}
