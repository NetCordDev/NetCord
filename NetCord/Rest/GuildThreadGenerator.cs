namespace NetCord.Rest;

internal class GuildThreadGenerator
{
    public static Dictionary<Snowflake, GuildThread> CreateThreads(JsonModels.JsonRestGuildThreadResult jsonThreads, RestClient client)
    {
        var threads = jsonThreads.Threads;
        var users = jsonThreads.Users;
        var threadsLength = threads.Length;
        var length = users.Length;
        Dictionary<Snowflake, GuildThread> result = new(threadsLength);
        int threadIndex = 0, userIndex = 0;
        while (userIndex < length)
        {
            var thread = threads[threadIndex++];
            var user = users[userIndex];
            if (thread.Id == user.ThreadId)
            {
                thread = thread with
                {
                    CurrentUser = user
                };
                userIndex++;
            }
            result.Add(thread.Id, (GuildThread)Channel.CreateFromJson(thread, client));
        }
        while (threadIndex < threadsLength)
        {
            var thread = threads[threadIndex++];
            result.Add(thread.Id, (GuildThread)Channel.CreateFromJson(thread, client));
        }
        return result;
    }

    public static IEnumerable<GuildThread> CreateThreads(JsonModels.JsonRestGuildThreadPartialResult jsonThreads, RestClient client)
    {
        var threads = jsonThreads.Threads;
        var users = jsonThreads.Users;
        var length = users.Length;
        int threadIndex = 0, userIndex = 0;
        while (userIndex < length)
        {
            var thread = threads[threadIndex++];
            var user = users[userIndex];
            if (thread.Id == user.ThreadId)
            {
                thread = thread with
                {
                    CurrentUser = user
                };
                userIndex++;
            }
            yield return (GuildThread)Channel.CreateFromJson(thread, client);
        }
        length = threads.Length;
        while (threadIndex < length)
        {
            var thread = threads[threadIndex++];
            yield return (GuildThread)Channel.CreateFromJson(thread, client);
        }
    }
}
