using System.Text;

using NetCord;
using NetCord.JsonModels;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace ServicesTest;

[TestClass]
public class ComponentInteractionServiceTests : ServiceTests<ComponentInteractionServiceTester>
{
}

public class ComponentInteractionServiceTester : ServiceTester
{
    public override bool SupportsBigInteger => true;

    public override bool SupportsReadOnlyMemoryChar => true;

    public override bool SupportsUser => false;

    private ComponentInteraction CreateInteraction(string customId, InteractionType interactionType)
    {
        JsonInteraction jsonModel = new()
        {
            Type = interactionType,
            Data = new()
            {
                ComponentType = ComponentType.Button,
                CustomId = customId,
            },
            User = new(),
            Channel = new(),
            Entitlements = [],
            Message = new()
            {
                Author = new(),
                MentionedUsers = [],
                Attachments = [],
                Embeds = [],
            },
        };

        return (ComponentInteraction)Interaction.CreateFromJson(jsonModel, null, (_, _, _, _, _) => Task.FromResult<InteractionCallbackResponse?>(null), _client.Rest);
    }

    private async ValueTask ExecuteWithArgumentsAsync(string customIdBase, string[] customIdArguments, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        ComponentInteractionService<ComponentInteractionContext> service = new();

        service.AddComponentInteraction(customIdBase, handler);

        var componentInteraction = service.GetComponentInteractions().Values.First();

        StringBuilder customIdBuilder = new(customIdBase);

        int providedArgumentCount = customIdArguments.Length;
        for (int i = 0; i < providedArgumentCount; i++)
        {
            customIdBuilder.Append(':');
            customIdBuilder.Append(customIdArguments[i]);
        }

        var interactionParameterCount = componentInteraction.Parameters.Count;
        for (int i = providedArgumentCount; i < interactionParameterCount; i++)
            customIdBuilder.Append(':');

        string customId = customIdBuilder.ToString();

        var interaction = CreateInteraction(customId,
                                            InteractionType.MessageComponent);
        ComponentInteractionContext context = new(interaction, _client);
        var result = await service.ExecuteAsync(context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for interaction with custom ID '{customId}'.\n{ex.Message}");
        }
    }

    public async ValueTask ExecuteRawAsync(string customId, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        ComponentInteractionService<ComponentInteractionContext> service = new();

        service.AddComponentInteraction(customId, handler);

        var interaction = CreateInteraction(customId,
                                            InteractionType.MessageComponent);
        ComponentInteractionContext context = new(interaction, _client);
        var result = await service.ExecuteAsync(context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for interaction with custom ID '{customId}'.\n{ex.Message}");
        }
    }

    public override ValueTask ExecuteNoArgumentsAsync(string commandName, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteWithArgumentsAsync(commandName, [], resultHandler, handler, services);
    }

    public override ValueTask ExecuteSingleArgumentAsync(string commandName, string argument, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteWithArgumentsAsync(commandName, [argument], resultHandler, handler, services);
    }
}
