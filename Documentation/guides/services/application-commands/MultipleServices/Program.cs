using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
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
await manager.CreateCommandsAsync(client.Rest, client.ApplicationId);

client.InteractionCreate += async interaction =>
{
    try
    {
        await (interaction switch
        {
            SlashCommandInteraction slashCommandInteraction => slashCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client)),
            MessageCommandInteraction messageCommandInteraction => messageCommandService.ExecuteAsync(new MessageCommandContext(messageCommandInteraction, client)),
            _ => throw new("Invalid interaction.")
        });
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
