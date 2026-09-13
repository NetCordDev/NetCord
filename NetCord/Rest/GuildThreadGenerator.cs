namespace NetCord.Rest;

internal static class GuildThreadGenerator
{
    public static IEnumerable<IGuildThread> CreateThreads(JsonModels.JsonRestGuildThreadResult jsonThreads, RestClient client)
    {
        var users = jsonThreads.Users.ToDictionary(u => u.ThreadId);
        return jsonThreads.Threads.Select(t =>
        {
            if (users.TryGetValue(t.Id, out var user))
                t.CurrentUser = user;

            return ChannelFactory.CreateGuildThread(t, client);
        });
    }
}
