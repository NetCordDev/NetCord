namespace NetCord;

internal static class EntityMenuHelper
{
    public static User[] GetUserValues(string[] selectedValues, InteractionResolvedData resolvedData)
    {
        var users = resolvedData.Users;
        return selectedValues.Select(v => users![Snowflake.Parse(v)]).ToArray();
    }

    public static Role[] GetRoleValues(string[] selectedValues, InteractionResolvedData resolvedData)
    {
        var roles = resolvedData.Roles;
        return selectedValues.Select(v => roles![Snowflake.Parse(v)]).ToArray();
    }

    public static Mentionable[] GetMentionableValues(string[] selectedValues, InteractionResolvedData resolvedData)
    {
        var users = resolvedData.Users;
        var roles = resolvedData.Roles;
        return users is null
            ? roles is null
                ? []
                : selectedValues.Select(v => new Mentionable.Role(roles[Snowflake.Parse(v)])).ToArray()
            : roles is null
                ? selectedValues.Select(v => new Mentionable.User(users[Snowflake.Parse(v)])).ToArray()
                : selectedValues.Select(v =>
                {
                    var id = Snowflake.Parse(v);
                    return (Mentionable)(users.TryGetValue(id, out var user) ? new Mentionable.User(user) : new Mentionable.Role(roles[id]));
                }).ToArray();
    }

    public static Channel[] GetChannelValues(string[] selectedValues, InteractionResolvedData resolvedData)
    {
        var channels = resolvedData.Channels;
        return selectedValues.Select(v => channels![Snowflake.Parse(v)]).ToArray();
    }
}
