using Microsoft.Extensions.DependencyInjection;

namespace HostingTest;

public abstract class SingleClassMultipleHandlersSupportedHandlersTests<TTester>(TestContext context) : HandlersTests<TTester>(context) where TTester : ISingleClassMultipleHandlersSupportedHandlersTester
{
    // Single Class Multiple Handlers
    [TestMethod]
    public async ValueTask ClassSingletonSingleClassMultipleHandlersSupported()
    {
        var (rateLimitedCounter, applicationCommandPermissionsUpdateCounter) = await ClassSingleClassMultipleHandlersSupportedAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, rateLimitedCounter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, applicationCommandPermissionsUpdateCounter.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassTransientOrScopedSingleClassMultipleHandlersSupportedAsync(ServiceLifetime lifetime)
    {
        var (rateLimitedCounter, applicationCommandPermissionsUpdateCounter) = await ClassSingleClassMultipleHandlersSupportedAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(rateLimitedCounter.HandlerCount + applicationCommandPermissionsUpdateCounter.HandlerCount, rateLimitedCounter.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");

        Assert.AreEqual(applicationCommandPermissionsUpdateCounter.HandlerCount + rateLimitedCounter.HandlerCount, applicationCommandPermissionsUpdateCounter.ConstructorCount, "Handler constructor was not called the same number of times as both handlers were called.");
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
}

