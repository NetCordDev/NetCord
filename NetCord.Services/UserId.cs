namespace NetCord.Services;

public class UserId : Entity
{
    public override Snowflake Id { get; }
    public User? User { get; }

    public UserId(Snowflake id, User? user) : this(id)
    {
        User = user;
    }

    public UserId(Snowflake id)
    {
        Id = id;
    }

    public override string? ToString() => $"<@{Id}>";
}
