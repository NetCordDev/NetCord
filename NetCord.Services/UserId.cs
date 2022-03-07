namespace NetCord.Services;

public class UserId : Entity
{
    public override DiscordId Id { get; }
    public User? User { get; }

    public UserId(DiscordId id, User? user) : this(id)
    {
        User = user;
    }

    public UserId(DiscordId id)
    {
        Id = id;
    }

    public override string? ToString() => User != null ? User.ToString() : Id.ToString();
}