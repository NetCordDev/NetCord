﻿using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

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
    }
};
var result = await slashCommandService.CreateCommandsAsync(client.Rest, token.Id);
await client.StartAsync();

await Task.Delay(-1);
