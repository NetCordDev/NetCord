namespace NetCord.Services;

public class Mentionable : Entity
{
    public override ulong Id { get; }
    public User? User { get; }
    public Role? Role { get; }
    public MentionableType Type { get; }

    private readonly bool _user;

    public Mentionable(User user)
    {
        Id = user.Id;
        User = user;
        Type = MentionableType.User;
        _user = true;
    }

    public Mentionable(Role role)
    {
        Id = role.Id;
        Role = role;
        Type = MentionableType.Role;
    }

    public override string ToString() => _user ? User!.ToString() : Role!.ToString();
}

public enum MentionableType : byte
{
    Role,
    User,
}
