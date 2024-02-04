using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

ApplicationCommandService<SlashCommandContext> slashCommandService = new();
ApplicationCommandService<MessageCommandContext> messageCommandService = new();

ApplicationCommandServiceManager manager = new();
manager.AddService(slashCommandService);
manager.AddService(messageCommandService);

var assembly = typeof(Program).Assembly;
slashCommandService.AddModules(assembly);
messageCommandService.AddModules(assembly);

client.InteractionCreate += async interaction =>
{
    var result = await (interaction switch
    {
        SlashCommandInteraction slashCommandInteraction => slashCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client)),
        MessageCommandInteraction messageCommandInteraction => messageCommandService.ExecuteAsync(new MessageCommandContext(messageCommandInteraction, client)),
        _ => throw new("Invalid interaction."),
    });

    if (result is not IFailResult failResult)
        return;

    try
    {
        await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    }
    catch
    {
    }
};

await manager.CreateCommandsAsync(client.Rest, client.Id);

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

await client.StartAsync();
await Task.Delay(-1);
