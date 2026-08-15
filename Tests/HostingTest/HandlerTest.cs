using Microsoft.Extensions.DependencyInjection;

namespace HostingTest;

public abstract class HandlerTest<TTester>(TestContext context) where TTester : IHandlerTester
{
    protected const int HandlerCallCount = 10;

    protected readonly TestContext _context = context;

    // Class Factory
    [TestMethod]
    public async ValueTask ClassFactorySingletonGetsCalledAndIsSingleton()
    {
        var counter = await ClassFactoryGetsCalledAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryGetsCalledAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");
    }

    private async ValueTask<Counter> ClassFactoryGetsCalledAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Class Factory Scopes
    [TestMethod]
    [DataRow(ServiceLifetime.Singleton)]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public ValueTask ClassFactoryIsOrIsNotScopedAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        return Helper.RunUntilAsync(() => TTester.CreateClassFactoryScopedTestHost(counter, lifetime),
                                    () => counter.HandlerCount >= HandlerCallCount,
                                    _context.CancellationToken);
    }

    // Class No Factory
    [TestMethod]
    public async ValueTask ClassSingletonGetsCalledAndIsSingleton()
    {
        var counter = await ClassGetsCalledAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassTransientOrScopedGetsCalledAndIsNotSingleton(ServiceLifetime lifetime)
    {
        var counter = await ClassGetsCalledAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");
    }

    private async ValueTask<Counter> ClassGetsCalledAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Class No Factory Scopes
    [TestMethod]
    [DataRow(ServiceLifetime.Singleton)]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public ValueTask ClassIsOrIsNotScopedAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        return Helper.RunUntilAsync(() => TTester.CreateClassScopedTestHost(counter, lifetime),
                                    () => counter.HandlerCount >= HandlerCallCount,
                                    _context.CancellationToken);
    }

    // Factory Disposable
    [TestMethod]
    public async ValueTask ClassFactoryDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeCount, "Handler Dispose was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryDisposableTransientOrScopedGetsDisposed(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeCount, "Handler Dispose was not called the same number of times as the handler was called.");
    }

    private async ValueTask<DisposableCounter> ClassFactoryDisposableAsync(ServiceLifetime lifetime)
    {
        DisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Hidden Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeCount, "Handler Dispose was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryHiddenDisposableTransientOrScopedGetsDisposed(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeCount, "Handler Dispose was not called the same number of times as the handler was called.");
    }

    private async ValueTask<DisposableCounter> ClassFactoryHiddenDisposableAsync(ServiceLifetime lifetime)
    {
        DisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryHiddenDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryAsyncDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryAsyncDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same number of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryAsyncDisposableAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryAsyncDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Hidden Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenAsyncDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryHiddenAsyncDisposableTransientAndScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenAsyncDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same number of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryHiddenAsyncDisposableAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryHiddenAsyncDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Disposable and Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryDisposableAndAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryDisposableAndAsyncDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryDisposableAndAsyncDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same number of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryDisposableAndAsyncDisposableAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryDisposableAndAsyncDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Factory Hidden Disposable and Async Disposable
    [TestMethod]
    public async ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableSingletonGetsDisposed()
    {
        var counter = await ClassFactoryHiddenDisposableAndAsyncDisposableAsync(ServiceLifetime.Singleton).ConfigureAwait(false);

        Assert.AreEqual(1, counter.ConstructorCount, "Handler constructor was not called exactly once.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(1, counter.DisposeAsyncCount, "Handler DisposeAsync was not called exactly once.");
    }

    [TestMethod]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask ClassFactoryHiddenDisposableAndAsyncDisposableTransientOrScopedGetsDisposedAsync(ServiceLifetime lifetime)
    {
        var counter = await ClassFactoryHiddenDisposableAndAsyncDisposableAsync(lifetime).ConfigureAwait(false);

        Assert.AreEqual(counter.HandlerCount, counter.ConstructorCount, "Handler constructor was not called the same number of times as the handler was called.");

        Assert.AreEqual(0, counter.DisposeCount, "Handler Dispose was called when it should not have been.");

        Assert.AreEqual(counter.HandlerCount, counter.DisposeAsyncCount, "Handler DisposeAsync was not called the same number of times as the handler was called.");
    }

    private async ValueTask<AsyncDisposableCounter> ClassFactoryHiddenDisposableAndAsyncDisposableAsync(ServiceLifetime lifetime)
    {
        AsyncDisposableCounter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        return counter;
    }

    // Delegate
    [TestMethod]
    [DataRow(ServiceLifetime.Singleton)]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask DelegateGetsCalledAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateDelegateTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }

    // Delegate with parameters
    [TestMethod]
    [DataRow(ServiceLifetime.Singleton)]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask DelegateWithParametersGetsCalledAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateDelegateWithParametersTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }

    // Delegate Scopes
    [TestMethod]
    [DataRow(ServiceLifetime.Singleton)]
    [DataRow(ServiceLifetime.Transient)]
    [DataRow(ServiceLifetime.Scoped)]
    public async ValueTask DelegateIsOrIsNotScopedAsync(ServiceLifetime lifetime)
    {
        Counter counter = new();

        await Helper.RunUntilAsync(() => TTester.CreateDelegateScopedTestHost(counter, lifetime),
                                   () => counter.HandlerCount >= HandlerCallCount,
                                   _context.CancellationToken).ConfigureAwait(false);

        Assert.AreEqual(0, counter.ConstructorCount);
    }
}
