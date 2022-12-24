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
        GatewayClient client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("OriginalTurboBoat")!), new()
        {
            Intents = GatewayIntent.All,
        });
        Guild? guild = null;
        TaskCompletionSource completionSource = new();
        client.GuildCreate += g =>
        {
            if (g.GuildId == 819892011364122624)
            {
                if (g.Guild == null)
                    throw new($"{nameof(g.Guild)} was null");
                client.Dispose();
                guild = g.Guild;
                completionSource.SetResult();
            }
            return default;
        };
        await client.StartAsync().ConfigureAwait(false);
        await completionSource.Task.ConfigureAwait(false);
        var model = ((IJsonModel<JsonGuild>)guild!).JsonModel;
        var json = JsonSerializer.Serialize(model, JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild);
        var deserialized = JsonSerializer.Deserialize(json, JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild)!;
        Guild newGuild = new(deserialized, client.Rest);

        model.WithDeepEqual(deserialized).Assert();
    }
}
