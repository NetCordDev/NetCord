namespace NetCord.Services.ApplicationCommands;

public class Mentionable
{
    public GuildRole? Role { get; }
    public User? User { get; }
    public MentionableType Type { get; }

    public Mentionable(GuildRole role)
    {
        Role = role;
        Type = MentionableType.Role;
    }

    public Mentionable(User user)
    {
        User = user;
        Type = MentionableType.User;
    }
}

public enum MentionableType
{
    Role,
    User,
}