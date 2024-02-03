using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

ApplicationCommandService<SlashCommandContext> applicationCommandService = new();
applicationCommandService.AddSlashCommand("ping", "Ping!", () => "Pong!");
applicationCommandService.AddModules(typeof(Program).Assembly);

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
await client.StartAsync();
await client.ReadyAsync;
await applicationCommandService.CreateCommandsAsync(client.Rest, client.Id);

client.InteractionCreate += async interaction =>
{
    if (interaction is not SlashCommandInteraction slashCommandInteraction)
        return;

    var result = await applicationCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client));

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

await Task.Delay(-1);
