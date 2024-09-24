using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;
using NetCord.Test.Sharded;

CommandService<CommandContext> commandService = new();
commandService.AddModule<ExampleModule>();
commandService.AddCommand(["pong"], ReplyMessageProperties () => "ping!");

var configuration = ApplicationCommandServiceConfiguration<SlashCommandContext>.Default;
//configuration = configuration with
//{
//    TypeReaders = configuration.TypeReaders.Add(typeof(Permissions), new PermissionsTypeReader()),
//};

ApplicationCommandService<SlashCommandContext, AutocompleteInteractionContext> slashCommandService = new(configuration);
slashCommandService.AddModule<ExampleModule2>();
slashCommandService.AddSlashCommand("button", "Button!", () =>
{
    return new InteractionMessageProperties()
    {
        Components = [new ActionRowProperties([new ButtonProperties("button", "Button!", ButtonStyle.Primary)])],
    };
});

ComponentInteractionService<ButtonInteractionContext> buttonInteractionService = new();
buttonInteractionService.AddInteraction("button", () => "XD");

BotToken token = new(Environment.GetEnvironmentVariable("token")!);
ShardedGatewayClient client = new(token, new()
{
    ShardCount = 1,
    IntentsFactory = shard => GatewayIntents.All,
    PresenceFactory = shard => new(UserStatusType.Online)
    {
        Activities =
        [
            new("c", UserActivityType.Custom)
            {
                State = $"Shard #{shard.Id}",
            },
        ],
    },
});
client.Log += (client, message) =>
{
    var shard = client.Shard.GetValueOrDefault();
    Console.WriteLine($"#{shard.Id}\t{message}");
    return default;
};
client.MessageCreate += async (client, message) =>
{
    if (message.Author.IsBot)
        return;

    if (message.Content.StartsWith('!'))
    {
        try
        {
            await commandService.ExecuteAsync(1, new(message, client));
        }
        catch (Exception ex)
        {
            await message.ReplyAsync(ex.Message);
        }
    }
};
client.InteractionCreate += async (client, interaction) =>
{
    switch (interaction)
    {
        case SlashCommandInteraction slashCommandInteraction:
            {
                try
                {
                    await slashCommandService.ExecuteAsync(new(slashCommandInteraction, client));
                }
                catch (Exception ex)
                {
                    await slashCommandInteraction.SendResponseAsync(InteractionCallback.Message(ex.Message));
                }

                break;
            }

        case AutocompleteInteraction autocompleteInteraction:
            {
                try
                {
                    await slashCommandService.ExecuteAutocompleteAsync(new(autocompleteInteraction, client));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                break;
            }

        case ButtonInteraction buttonInteraction:
            {
                try
                {
                    await buttonInteractionService.ExecuteAsync(new(buttonInteraction, client));
                }
                catch (Exception ex)
                {
                    await buttonInteraction.SendResponseAsync(InteractionCallback.Message(ex.Message));
                }

                break;
            }
    }
};
var result = await slashCommandService.CreateCommandsAsync(client.Rest, token.Id);
await client.StartAsync();

await Task.Delay(-1);
