using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal static class GuildThreadGenerator
{
    public static IEnumerable<GuildThread> CreateThreads(JsonChannel[] threads, JsonThreadUser[] users, RestClient client)
    {
        var usersMap = users.ToDictionary(u => u.ThreadId);

        return threads.Select(thread =>
        {
            if (usersMap.TryGetValue(thread.Id, out var user))
                thread.CurrentUser = user;

            return GuildThread.CreateFromJson(thread, client);
        });
    }
}
