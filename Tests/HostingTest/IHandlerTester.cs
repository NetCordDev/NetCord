using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostingTest;

public interface IHandlerTester
{
    public static abstract IHost CreateClassTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassScopedTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryScopedTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryHiddenDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryHiddenAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateDelegateTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateDelegateWithParametersTestHost(Counter counter, ServiceLifetime lifetime);

    public static abstract IHost CreateDelegateScopedTestHost(Counter counter, ServiceLifetime lifetime);
}
