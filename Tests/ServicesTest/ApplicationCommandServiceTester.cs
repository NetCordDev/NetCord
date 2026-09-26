using NetCord;
using NetCord.JsonModels;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ServicesTest;

public sealed class ApplicationCommandServiceTester : ServiceTester
{
    public override bool SupportsBigInteger => false;

    public override bool SupportsReadOnlyMemoryChar => false;

    public override bool SupportsUser => true;

    private ApplicationCommandInteraction CreateInteraction(string name, string?[] argumentValues, IReadOnlyList<string> argumentNames, ApplicationCommandType type)
    {
        JsonInteraction jsonModel = new()
        {
            Type = InteractionType.ApplicationCommand,
            Data = new()
            {
                Type = type,
                Name = name,
                Options =
                [..
                    argumentValues.Index().Where(x => x.Item is not null).Select(x =>
                        new JsonApplicationCommandInteractionDataOption()
                        {
                            Name = argumentNames[x.Index],
                            Type = ApplicationCommandOptionType.String,
                            Value = x.Item,
                        }
                    )
                ],
            },
            User = new(),
            Channel = new(),
            Entitlements = [],
        };

        return (ApplicationCommandInteraction)Interaction.CreateFromJson(jsonModel, null, (_, _, _, _, _) => Task.FromResult<InteractionCallbackResponse?>(null), _client.Rest);
    }

    public async ValueTask ExecuteAsync(string commandName, string?[] arguments, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        NameAndTypeApplicationCommandServiceStorage<ApplicationCommandContext> storage = new();

        var config = ApplicationCommandServiceConfiguration<ApplicationCommandContext>.Default with
        {
            Storage = storage,
        };

        ApplicationCommandService<ApplicationCommandContext> service = new(config);

        service.AddSlashCommand(new(commandName, commandName, handler));

        var command = (SlashCommandInfo<ApplicationCommandContext>)storage.GetCommands()[0];

        var interaction = CreateInteraction(commandName,
                                            arguments,
                                            [.. command.Parameters.Select(p => p.Name)],
                                            ApplicationCommandType.ChatInput);
        ApplicationCommandContext context = new(interaction, _client);
        var result = await service.ExecuteAsync(context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for interaction '{string.Join(", ", [commandName, .. arguments])}'.\n{ex.Message}");
        }
    }

    public override ValueTask ExecuteNoArgumentsAsync(string commandName, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync(commandName, [], resultHandler, handler, services);
    }

    public override ValueTask ExecuteSingleArgumentAsync(string commandName, string? argument, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync(commandName, [argument], resultHandler, handler, services);
    }
}
