namespace Nerosoft.Euonia.Collections;

/// <summary>
/// A tree structural object.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class TreeView<TEntity>
{
    /// <summary>
    /// Gets or sets the entity.
    /// </summary>
    /// <value>The entity.</value>
    public virtual TEntity Entity { get; set; }

    /// <summary>
    /// Gets or sets the children.
    /// </summary>
    /// <value>The children.</value>
    public virtual ICollection<TreeView<TEntity>> Children { get; set; }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public virtual IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the <see cref="object"/> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NullReferenceException">
    /// </exception>
    public virtual object this[string key]
    {
        get
        {
            if (Properties == null)
            {
                throw new NullReferenceException();
            }

            return Properties[key];
        }
        set
        {
            if (Properties == null)
            {
                throw new NullReferenceException();
            }

            if (!Properties.ContainsKey(key))
            {
                Properties.Add(key, value);
            }
            else
            {
                Properties[key] = value;
            }
        }
    }
}