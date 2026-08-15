namespace HostingTest;

public class Counter
{
    public int ConstructorCount { get; set; }

    public int HandlerCount { get; set; }
}

public class DisposableCounter : Counter
{
    public int DisposeCount { get; set; }
}

public class AsyncDisposableCounter : DisposableCounter
{
    public int DisposeAsyncCount { get; set; }
}
