using System.Runtime.CompilerServices;

using NetCord.Logging;

namespace NetCord.Rest;

internal sealed class TypingScope : IDisposable
{
    private const long DefaultInterval = 7 * TimeSpan.TicksPerSecond;

    private ITimer? _timer;
    private CancellationTokenSource _tokenSource;

    // 0 - not triggered, 1 - triggered, 2 - disposed
    private byte _state;

    private byte _disposed;

    public TypingScope()
    {
        _tokenSource = new();
    }

    private static async void TriggerTypingState(Tuple<TypingScope, RestClient, ulong, RestRequestProperties?, CancellationToken> state)
    {
        var (scope, client, channelId, properties, cancellationToken) = state;

        if (Interlocked.CompareExchange(ref scope._state, 1, 0) is not 0)
            return;

        try
        {
            await client.TriggerTypingStateAsync(channelId, properties, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            client.Log(LogLevel.Error, channelId, ex, static (s, e) => $"An error occurred while triggering typing state for channel ID '{s}'{Environment.NewLine}{e}");
        }

        if (Interlocked.CompareExchange(ref scope._state, 0, 1) is 2)
            scope._tokenSource.Dispose();
    }

    internal async Task StartAsync(RestClient client, ulong channelId, TypingScopeProperties? scopeProperties, RestRequestProperties? properties, CancellationToken cancellationToken)
    {
        await client.TriggerTypingStateAsync(channelId, properties, cancellationToken).ConfigureAwait(false);

        var timeProvider = scopeProperties?.TimeProvider ?? TimeProvider.System;

        TimerCallback callback = timeProvider == TimeProvider.System
            ? static s => TriggerTypingState(Unsafe.As<Tuple<TypingScope, RestClient, ulong, RestRequestProperties?, CancellationToken>>(s!))
            : static s => TriggerTypingState((Tuple<TypingScope, RestClient, ulong, RestRequestProperties?, CancellationToken>)s!);

        var interval = scopeProperties?.Interval ?? new(DefaultInterval);

        using (ExecutionContext.SuppressFlow())
            _timer = timeProvider.CreateTimer(callback, Tuple.Create(this, client, channelId, properties, _tokenSource.Token), interval, interval);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) is not 0)
            return;

        _timer?.Dispose();

        _tokenSource.Cancel();

        if (Interlocked.Exchange(ref _state, 2) is 0)
            _tokenSource.Dispose();
    }
}
