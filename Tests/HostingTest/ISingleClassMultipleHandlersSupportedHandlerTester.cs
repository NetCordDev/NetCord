using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostingTest;

public interface ISingleClassMultipleHandlersSupportedHandlerTester : IHandlerTester
{
    public static abstract IHost CreateClassSingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime);
}
