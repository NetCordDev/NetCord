using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

ComponentInteractionService<ButtonInteractionContext> interactionService = new();
interactionService.AddModules(typeof(Program).Assembly);

client.InteractionCreate += async interaction =>
{
    if (interaction is not ButtonInteraction buttonInteraction)
        return;

    var result = await interactionService.ExecuteAsync(new ButtonInteractionContext(buttonInteraction, client));

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

await client.StartAsync();
await Task.Delay(-1);
