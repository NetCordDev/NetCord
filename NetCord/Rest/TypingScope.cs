using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

using NetCord.Logging;

namespace NetCord.Rest;

internal class AsyncTypingScope : TypingScope, IValueTaskSource<IDisposable>
{
    private readonly CancellationToken _callerCancellationToken;
    private ManualResetValueTaskSourceCore<IDisposable> _valueTaskSource;

    private static TimerCallback GetCallback(TimeProvider timeProvider)
    {
        return timeProvider == TimeProvider.System
                ? static s => Callback(Unsafe.As<AsyncTypingScope>(s!))
                : static s => Callback((AsyncTypingScope)s!);
    }

    public short Version => _valueTaskSource.Version;

    public AsyncTypingScope(RestClient client,
                            ulong channelId,
                            TypingScopeProperties? scopeProperties,
                            RestRequestProperties? properties,
                            CancellationToken cancellationToken)
    {
        _callerCancellationToken = cancellationToken;
        _valueTaskSource.RunContinuationsAsynchronously = true;

        var timeProvider = scopeProperties?.TimeProvider ?? TimeProvider.System;

        Initialize(client,
                   channelId,
                   properties,
                   GetCallback(timeProvider),
                   scopeProperties,
                   timeProvider);
    }

    private static async void Callback(AsyncTypingScope scope)
    {
        if (!CallbackTryEnter(scope))
            return;

        var isTaskPending = scope._valueTaskSource.GetStatus(scope._valueTaskSource.Version) is ValueTaskSourceStatus.Pending;

        try
        {
            await TriggerTypingAsync(scope, isTaskPending ? scope._callerCancellationToken : scope._tokenSource.Token).ConfigureAwait(false);

            if (isTaskPending)
                scope._valueTaskSource.SetResult(scope);
        }
        catch (Exception ex)
        {
            if (isTaskPending)
            {
                scope.Dispose();
                scope._valueTaskSource.SetException(ex);
            }
            else if (ex is not OperationCanceledException)
                CallbackLogError(scope, ex);
        }

        CallbackExit(scope);
    }

    public IDisposable GetResult(short token) => _valueTaskSource.GetResult(token);

    public ValueTaskSourceStatus GetStatus(short token) => _valueTaskSource.GetStatus(token);

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        _valueTaskSource.OnCompleted(continuation, state, token, flags);
    }
}

internal class TypingScope : IDisposable
{
    protected const long DefaultInterval = 7 * TimeSpan.TicksPerSecond;

    private RestClient _client;
    private ulong _channelId;
    private RestRequestProperties? _properties;

    private ITimer _timer;
    protected CancellationTokenSource _tokenSource;

    // 0 - not triggered, 1 - triggered, 2 - disposed
    protected byte _state;

    private byte _disposed;

    private static TimerCallback GetCallback(TimeProvider timeProvider)
    {
        return timeProvider == TimeProvider.System
                ? static s => Callback(Unsafe.As<TypingScope>(s!))
                : static s => Callback((TypingScope)s!);
    }

    public TypingScope(RestClient client,
                       ulong channelId,
                       TypingScopeProperties? scopeProperties,
                       RestRequestProperties? properties)
    {
        var timeProvider = scopeProperties?.TimeProvider ?? TimeProvider.System;

        Initialize(client,
                   channelId,
                   properties,
                   GetCallback(timeProvider),
                   scopeProperties,
                   timeProvider);
    }

#pragma warning disable CS8618
    protected TypingScope()
#pragma warning restore CS8618
    {
    }

    [MemberNotNull(nameof(_client), nameof(_tokenSource), nameof(_timer))]
    protected void Initialize(RestClient client,
                              ulong channelId,
                              RestRequestProperties? properties,
                              TimerCallback callback,
                              TypingScopeProperties? scopeProperties,
                              TimeProvider timeProvider)
    {
        _client = client;
        _channelId = channelId;
        _properties = properties;
        _tokenSource = new();

        try
        {
            using (ExecutionContext.SuppressFlow())
                _timer = timeProvider.CreateTimer(callback,
                                                  this,
                                                  TimeSpan.Zero,
                                                  scopeProperties?.Interval ?? new(DefaultInterval));
        }
        catch
        {
            _tokenSource.Dispose();
            throw;
        }
    }

    private static async void Callback(TypingScope scope)
    {
        if (!CallbackTryEnter(scope))
            return;

        try
        {
            await TriggerTypingAsync(scope, scope._tokenSource.Token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                CallbackLogError(scope, ex);
        }

        CallbackExit(scope);
    }

    protected static Task TriggerTypingAsync(TypingScope scope, CancellationToken cancellationToken)
    {
        return scope._client.TriggerTypingAsync(scope._channelId, scope._properties, cancellationToken);
    }

    protected static bool CallbackTryEnter(TypingScope scope)
    {
        return Interlocked.CompareExchange(ref scope._state, 1, 0) is 0;
    }

    protected static void CallbackLogError(TypingScope scope, Exception ex)
    {
        scope._client.Log(LogLevel.Error, scope._channelId, ex, static (s, e) =>
        {
            return $"An error occurred while triggering typing for channel ID '{s}'.{Environment.NewLine}{e}";
        });
    }

    protected static void CallbackExit(TypingScope scope)
    {
        if (Interlocked.CompareExchange(ref scope._state, 0, 1) is 2)
            scope._tokenSource.Dispose();
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) is not 0)
            return;

        _timer.Dispose();

        _tokenSource.Cancel();

        if (Interlocked.Exchange(ref _state, 2) is 0)
            _tokenSource.Dispose();
    }
}
