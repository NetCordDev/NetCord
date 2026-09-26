using NetCord;
using NetCord.Gateway;

namespace ServicesTest;

public abstract class ServiceTester
{
    protected readonly GatewayClient _client = new(new BotToken("ODAzMzc3MjcwODc4MTA5NzI2.tHis.IS.not.A.ReAl.tOkeN"));

    public abstract bool SupportsBigInteger { get; }

    public abstract bool SupportsReadOnlyMemoryChar { get; }

    public abstract bool SupportsUser { get; }

    public abstract ValueTask ExecuteNoArgumentsAsync(string commandName, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null);

    public abstract ValueTask ExecuteSingleArgumentAsync(string commandName, string argument, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null);
}
