namespace HostingTest;

internal class Counter
{
    public int ConstructorCount { get; set; }

    public int HandlerCount { get; set; }
}

internal class DisposableCounter : Counter
{
    public int DisposeCount { get; set; }
}

internal class AsyncDisposableCounter : DisposableCounter
{
    public int DisposeAsyncCount { get; set; }
}
