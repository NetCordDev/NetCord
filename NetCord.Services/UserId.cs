namespace NetCord.Services;

public class UserId : Entity
{
    public override ulong Id { get; }
    public User? User { get; }

    public UserId(ulong id, User? user) : this(id)
    {
        User = user;
    }

    public UserId(ulong id)
    {
        Id = id;
    }

    public override string? ToString() => $"<@{Id}>";
}
