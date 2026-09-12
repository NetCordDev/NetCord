using System.ComponentModel;

namespace NetCord;

public interface IPartialRole
{

    ulong Id { get; }

    string Name { get; }

    RolePosition Position { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    Color Color { get; }

    RoleColors Colors { get; }

    int RawPosition { get; }

    string? IconHash { get; }

    string? UnicodeEmoji { get; }

}
