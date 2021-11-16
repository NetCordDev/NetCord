namespace NetCord.Commands;

public class UserId : Entity
{
    public override DiscordId Id { get; }
    public GuildUser? User { get; }

    public UserId(DiscordId id, GuildUser? user)
    {
        Id = id;
        User = user;
    }

    public UserId(DiscordId id)
    {
        Id = id;
    }

    public override string ToString() => User == null ? User.ToString() : Id.ToString();
}