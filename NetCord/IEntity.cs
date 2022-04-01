namespace NetCord;

public interface IEntity
{
    /// <summary>
    /// Returns the unique identifier for this object.
    /// </summary>
    public Snowflake Id { get; }
}