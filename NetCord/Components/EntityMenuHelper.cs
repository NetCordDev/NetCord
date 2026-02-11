namespace NetCord;

internal static class EntityMenuHelper
{
    public static User[] GetUserValues(IEnumerable<ulong> selectedValues, InteractionResolvedData resolvedData)
    {
        var users = resolvedData.Users;
        return selectedValues.Select(v => users![v]).ToArray();
    }

    public static Role[] GetRoleValues(IEnumerable<ulong> selectedValues, InteractionResolvedData resolvedData)
    {
        var roles = resolvedData.Roles;
        return selectedValues.Select(v => roles![v]).ToArray();
    }

    public static Mentionable[] GetMentionableValues(IEnumerable<ulong> selectedValues, InteractionResolvedData resolvedData)
    {
        var users = resolvedData.Users;
        var roles = resolvedData.Roles;
        return users is null
            ? roles is null
                ? []
                : selectedValues.Select(v => new Mentionable.Role(roles[v])).ToArray()
            : roles is null
                ? selectedValues.Select(v => new Mentionable.User(users[v])).ToArray()
                : selectedValues.Select(v =>
                {
                    return (Mentionable)(users.TryGetValue(v, out var user) ? new Mentionable.User(user) : new Mentionable.Role(roles[v]));
                }).ToArray();
    }

    public static Channel[] GetChannelValues(IEnumerable<ulong> selectedValues, InteractionResolvedData resolvedData)
    {
        var channels = resolvedData.Channels;
        return selectedValues.Select(v => channels![v]).ToArray();
    }
}
