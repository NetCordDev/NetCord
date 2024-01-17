namespace NetCord.Rest;

internal static class GuildThreadGenerator
{
    public static Dictionary<ulong, GuildThread> CreateThreads(JsonModels.JsonRestGuildThreadResult jsonThreads, RestClient client)
    {
        var users = jsonThreads.Users.ToDictionary(u => u.ThreadId);
        return jsonThreads.Threads.ToDictionary(t => t.Id, t =>
        {
            if (users.TryGetValue(t.Id, out var user))
                t.CurrentUser = user;

            return GuildThread.CreateFromJson(t, client);
        });
    }

    public static IEnumerable<GuildThread> CreateThreads(JsonModels.JsonRestGuildThreadPartialResult jsonThreads, RestClient client)
    {
        var users = jsonThreads.Users.ToDictionary(u => u.ThreadId);
        foreach (var t in jsonThreads.Threads)
        {
            if (users.TryGetValue(t.Id, out var user))
                t.CurrentUser = user;

            yield return GuildThread.CreateFromJson(t, client);
        }
    }
}
