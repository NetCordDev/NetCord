using NetCord;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfig()
{
    Intents = default,
});

ApplicationCommandService<SlashCommandContext> slashCommandService = new();
ApplicationCommandService<MessageCommandContext> messageCommandService = new();

ApplicationCommandServiceManager manager = new();
manager.AddService(slashCommandService);
manager.AddService(messageCommandService);

var assembly = System.Reflection.Assembly.GetEntryAssembly()!;
slashCommandService.AddModules(assembly);
messageCommandService.AddModules(assembly);

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
await client.StartAsync();
await client.ReadyAsync;
await manager.CreateCommandsAsync(client.Rest, client.ApplicationId!.Value);

client.InteractionCreate += async interaction =>
{
    try
    {
        switch (interaction)
        {
            case SlashCommandInteraction slashCommandInteraction:
                await slashCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client));
                break;
            case MessageCommandInteraction messageCommandInteraction:
                await messageCommandService.ExecuteAsync(new MessageCommandContext(messageCommandInteraction, client));
                break;
        }
    }
    catch (Exception ex)
    {
        try
        {
            await interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"Error: {ex.Message}"));
        }
        catch
        {
        }
    }
};
await Task.Delay(-1);