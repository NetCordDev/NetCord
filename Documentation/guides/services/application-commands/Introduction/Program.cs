using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration
{
    Intents = default,
    Logger = new ConsoleLogger(),
});

// Create the application command service
ApplicationCommandService<ApplicationCommandContext> applicationCommandService = new();

// Add commands using minimal APIs
applicationCommandService.AddSlashCommand(new SlashCommandBuilder("ping", "Ping!", () => "Pong!"));
applicationCommandService.AddUserCommand(new UserCommandBuilder("Username", (User user) => user.Username));
applicationCommandService.AddMessageCommand(new MessageCommandBuilder("Length", (RestMessage message) => message.Content.Length.ToString()));

// Add commands from modules
applicationCommandService.AddModules(typeof(Program).Assembly);

// Add the handler to handle interactions
client.InteractionCreate += async interaction =>
{
    // Check if the interaction is an application command interaction
    if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
        return;

    // Execute the command
    var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, client));

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

// Register the commands so that you can use them in the Discord client
await applicationCommandService.RegisterCommandsAsync(client.Rest, client.Id);

await client.StartAsync();
await Task.Delay(-1);
