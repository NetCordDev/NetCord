using Microsoft.Extensions.DependencyInjection;

namespace HostingTest;

public abstract class SingleClassMultipleHandlersSupportedHandlerTests<TTester>(TestContext context) : HandlerTests<TTester>(context) where TTester : ISingleClassMultipleHandlersSupportedHandlerTester
{
    // Single Class Multiple Handlers
    [TestMethod]
    public async ValueTask ClassSingletonSingleClassMultipleHandlersSupported()
    {
        var (counter1, counter2) = await ClassSingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter1.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter2.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        var (counter1, counter2) = await ClassSingleClassMultipleHandlersSupportedAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter1.HandlerCount + counter2.HandlerCount, counter1.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");

        Assert.AreEqual(counter2.HandlerCount + counter1.HandlerCount, counter2.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");
    }

    private async ValueTask<(Counter, Counter)> ClassSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        Counter counter1 = new();
        Counter counter2 = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassSingleMultipleHandlersTestHost(counter1, counter2, lifetime),
                                   () => counter1.HandlerCount >= HandlerCallCount && counter2.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return (counter1, counter2);
    }

    [TestMethod]
    public async ValueTask ClassFactorySingletonSingleClassMultipleHandlersSupported()
    {
        var (counter1, counter2) = await ClassFactorySingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter1.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter2.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        var (counter1, counter2) = await ClassFactorySingleClassMultipleHandlersSupportedAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter1.HandlerCount + counter2.HandlerCount, counter1.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");

        Assert.AreEqual(counter2.HandlerCount + counter1.HandlerCount, counter2.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");
    }

    private async ValueTask<(Counter, Counter)> ClassFactorySingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        Counter counter1 = new();
        Counter counter2 = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactorySingleMultipleHandlersTestHost(counter1, counter2, lifetime),
                                   () => counter1.HandlerCount >= HandlerCallCount && counter2.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return (counter1, counter2);
    }
}

