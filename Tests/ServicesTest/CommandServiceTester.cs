using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Services.Commands;

namespace ServicesTest;

public sealed class CommandServiceTesterSession
{
    public CommandServiceTesterSession(GatewayClient client)
    {
        _client = client;

        _service = new(_config = CommandServiceConfiguration<CommandContext>.Default);
    }

    private readonly GatewayClient _client;

    private readonly CommandServiceConfiguration<CommandContext> _config;

    private readonly CommandService<CommandContext> _service;

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

    public string GetCommandName(string command)
    {
        return command.Split([.. _config.ParameterSeparators])[0];
    }

    public void AddCommand(string commandName, Delegate handler, int priority = 0)
    {
        _service.AddCommand([commandName], handler, priority);
    }

    public async ValueTask ExecuteAsync(string command, ResultHandler resultHandler, IServiceProvider? services = null)
    {
        var message = CreateMessage(command);
        CommandContext context = new(message, _client);
        var result = await _service.ExecuteAsync(0, context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for command '{command}'.\n{ex.Message}");
        }
    }
}

public sealed class CommandServiceTester : ServiceTester
{
    public override bool SupportsBigInteger => true;

    public override bool SupportsReadOnlyMemoryChar => true;

    public override bool SupportsUser => true;

    public CommandServiceTesterSession StartSession()
    {
        return new CommandServiceTesterSession(_client);
    }

    public ValueTask ExecuteAsync(string command, ResultHandler resultHandler, Delegate handler, int priority = 0, IServiceProvider? services = null)
    {
        var session = StartSession();

        var commandName = session.GetCommandName(command);

        session.AddCommand(commandName, handler, priority);

        return session.ExecuteAsync(command, resultHandler, services);
    }

    public override ValueTask ExecuteNoArgumentsAsync(string commandName, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync(commandName, resultHandler, handler, services: services);
    }

    public override ValueTask ExecuteSingleArgumentAsync(string commandName, string argument, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        return ExecuteAsync($"{commandName} {argument}", resultHandler, handler, services: services);
    }
}
