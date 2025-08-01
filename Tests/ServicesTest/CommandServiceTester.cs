using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Services.Commands;

namespace ServicesTest;

public sealed class CommandServiceTester : ServiceTester
{
    public override bool SupportsBigInteger => true;

    public override bool SupportsReadOnlyMemoryChar => true;

    private Message CreateMessage(string content)
    {
        JsonMessage jsonModel = new()
        {
            Author = new(),
            MentionedUsers = [],
            Attachments = [],
            Embeds = [],
            Content = content,
        };

        return new(jsonModel, null, null, _client.Rest);
    }

    public async ValueTask ExecuteMultipleAsync(string[] commands, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        foreach (var command in commands)
            await ExecuteAsync(command, resultHandler, handler, services).ConfigureAwait(false);
    }

    public async ValueTask ExecuteAsync(string command, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        var config = CommandServiceConfiguration<CommandContext>.Default;

        CommandService<CommandContext> service = new(config);

        var commandName = command.Split([.. config.ParameterSeparators])[0];

        service.AddCommand([commandName], handler);

        var message = CreateMessage(command);
        CommandContext context = new(message, _client);
        var result = await service.ExecuteAsync(0, context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for command '{command}'.\n{ex.Message}");
        }
    }

    public override ValueTask ExecuteNoArgumentsAsync(string commandName, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync(commandName, resultHandler, handler, services);
    }

    public override ValueTask ExecuteSingleArgumentAsync(string commandName, string argument, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync($"{commandName} {argument}", resultHandler, handler, services);
    }
}
