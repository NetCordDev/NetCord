using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

ApplicationCommandService<UserCommandContext> applicationCommandService = new();
applicationCommandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
await client.StartAsync();
await client.ReadyAsync;
await applicationCommandService.CreateCommandsAsync(client.Rest, client.ApplicationId);

client.InteractionCreate += async interaction =>
{
    if (interaction is UserCommandInteraction userCommandInteraction)
    {
        try
        {
            await applicationCommandService.ExecuteAsync(new UserCommandContext(userCommandInteraction, client));
        }
        catch (Exception ex)
        {
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message($"Error: {ex.Message}"));
            }
            catch
            {
            }
        }
    }
};
await Task.Delay(-1);
