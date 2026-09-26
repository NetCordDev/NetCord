using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HandlersTest;

public interface ISingleClassMultipleHandlersSupportedHandlerTester : IHandlerTester
{
    public static abstract IHost CreateClassSingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactorySingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime);
}
