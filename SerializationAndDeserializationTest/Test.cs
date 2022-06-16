using System.Text.Json;

using DeepEqual.Syntax;

using NetCord;
using NetCord.Gateway;
using NetCord.JsonModels;

namespace SerializationAndDeserializationTest;

[TestClass]
public class Test
{
    [TestMethod]
    public async Task TestAsync()
    {
        GatewayClient client = new(Environment.GetEnvironmentVariable("token")!, TokenType.Bot, new()
        {
            Intents = GatewayIntent.Guilds,
        });
        Guild? guild = null;
        TaskCompletionSource completionSource = new();
        client.GuildCreate += g =>
        {
            client.Dispose();
            guild = g;
            completionSource.SetResult();
            return default;
        };
        await client.StartAsync().ConfigureAwait(false);
        await completionSource.Task.ConfigureAwait(false);
        var model = ((IJsonModel<JsonGuild>)guild!).JsonModel;
        var json = JsonSerializer.Serialize(model, Serialization.Options);
        var deserialized = JsonSerializer.Deserialize<JsonGuild>(json, Serialization.Options)!;
        Guild newGuild = new(deserialized, client.Rest);
        Console.WriteLine($"old: {model}");
        Console.WriteLine($"new: {deserialized}");

        Assert.IsTrue(model.IsDeepEqual(deserialized));
    }
}