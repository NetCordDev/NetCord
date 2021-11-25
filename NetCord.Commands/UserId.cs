namespace NetCord.Commands;

public class UserId : Entity
{
    public override DiscordId Id { get; }
    public GuildUser? User { get; }

    public UserId(DiscordId id, GuildUser? user) : this(id)
    {
        User = user;
    }

    public UserId(DiscordId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        Id = id;
    }

    public override string ToString() => User != null ? User.ToString() : Id.ToString();
}