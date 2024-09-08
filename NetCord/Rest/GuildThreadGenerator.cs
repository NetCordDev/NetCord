namespace NetCord.Rest;

internal static class GuildThreadGenerator
{
    public static IEnumerable<GuildThread> CreateThreads(JsonModels.JsonRestGuildThreadResult jsonThreads, RestClient client)
    {
        var users = jsonThreads.Users.ToDictionary(u => u.ThreadId);
        return jsonThreads.Threads.Select(t =>
        {
            if (users.TryGetValue(t.Id, out var user))
                t.CurrentUser = user;

            return GuildThread.CreateFromJson(t, client);
        });
    }
}
