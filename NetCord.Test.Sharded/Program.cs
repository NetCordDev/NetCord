using System.Reflection;

using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

CommandService<CommandContext> commandService = new();
commandService.AddModules(Assembly.GetEntryAssembly()!);

ShardedGatewayClient client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!), new()
{
    ShardCount = 3,
    IntentsFactory = shard => GatewayIntents.All,
    PresenceFactory = shard => new(UserStatusType.Online)
    {
        Activities = new UserActivityProperties[]
        {
            new("c", UserActivityType.Custom)
            {
                State = $"Shard #{shard.Id}",
            },
        },
    },
});
client.Log += (client, message) =>
{
    var shard = client.Shard.GetValueOrDefault();
    Console.WriteLine($"#{shard.Id}\t{message}");
    return default;
};
client.MessageCreate += async (client, message) =>
{
    if (message.Author.IsBot)
        return;

    if (message.Content.StartsWith('!'))
    {
        try
        {
            await commandService.ExecuteAsync(1, new(message, client));
        }
        catch (Exception ex)
        {
            await message.ReplyAsync(ex.Message);
        }
    }
};

await client.StartAsync();

await Task.Delay(-1);
