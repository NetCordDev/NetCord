using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HandlersTest;

internal static class Helper
{
    public const int DelayMilliseconds = 50;

    public static HostApplicationBuilder CreateBuilder()
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);

        builder.ConfigureContainer(new DefaultServiceProviderFactory(new()
        {
            ValidateScopes = true,
            ValidateOnBuild = true,
        }));

        builder.Logging.AddSimpleConsole();

        return builder;
    }

    public static async ValueTask RunUntilAsync(Func<IHost> getHost, Func<bool> completionCondition, CancellationToken cancellationToken)
    {
        using (var host = getHost())
        {
            await host.StartAsync(cancellationToken).ConfigureAwait(false);

            var start = Environment.TickCount64;

            while (true)
            {
                if (completionCondition())
                    break;

                if (Environment.TickCount64 - start > 10_000)
                    Assert.Fail("Handler was not called enough times for 10 seconds.");

                await Task.Delay(DelayMilliseconds, cancellationToken).ConfigureAwait(false);
            }

            await host.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        await Task.Delay(DelayMilliseconds, cancellationToken).ConfigureAwait(false);
    }

    public static (Counter, Counter) ExtractCounters(IEnumerable<Counter> counters)
    {
        var countersArray = counters.ToArray();
        Assert.HasCount(2, countersArray, "Expected exactly 2 counters.");

        var counter1 = countersArray[0];
        var counter2 = countersArray[1];

        return (counter1, counter2);
    }
}
