using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = default,
});

// Create the component interaction service with the button interaction context
ComponentInteractionService<ButtonInteractionContext> interactionService = new();

// Add a component interaction using minimal APIs
interactionService.AddInteraction("ping", () => "Pong!");

// Add component interactions from modules
interactionService.AddModules(typeof(Program).Assembly);

// Add the handler to handle interactions
client.InteractionCreate += async interaction =>
{
    // Check if the interaction is a button interaction
    if (interaction is not ButtonInteraction buttonInteraction)
        return;

    // Execute the interaction
    var result = await interactionService.ExecuteAsync(new ButtonInteractionContext(buttonInteraction, client));

    // Check if the execution failed
    if (result is not IFailResult failResult)
        return;

    // Return the error message to the user if the execution failed
    try
    {
        await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
    }
    catch
    {
    }
};

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

await client.StartAsync();
await Task.Delay(-1);
