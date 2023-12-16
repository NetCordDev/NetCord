using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.Interactions;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

InteractionService<ButtonInteractionContext> interactionService = new();
interactionService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);

client.InteractionCreate += async interaction =>
{
    if (interaction is ButtonInteraction buttonInteraction)
    {
        try
        {
            await interactionService.ExecuteAsync(new ButtonInteractionContext(buttonInteraction, client));
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
client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
await client.StartAsync();
await Task.Delay(-1);
