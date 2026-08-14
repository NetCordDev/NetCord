using Microsoft.Extensions.Hosting;

namespace HostingTest;

internal static class Helper
{
    public static async ValueTask RunUntilAsync(Func<HostApplicationBuilder> getBuilder, Func<bool> completionCondition, CancellationToken cancellationToken)
    {
        using (var host = getBuilder().Build())
        {
            await host.StartAsync(cancellationToken).ConfigureAwait(false);

            Assert.IsTrue(SpinWait.SpinUntil(completionCondition, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

            await host.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

