using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostingTest;

internal static class Helper
{
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

    public static async ValueTask RunUntilAsync(Func<HostApplicationBuilder> getBuilder, Func<bool> completionCondition, CancellationToken cancellationToken)
    {
        using (var host = getBuilder().Build())
        {
            await host.StartAsync(cancellationToken).ConfigureAwait(false);

            Assert.IsTrue(SpinWait.SpinUntil(completionCondition, TimeSpan.FromSeconds(10)), "Handler was not called enough times for 10 seconds.");

            await host.StopAsync(cancellationToken).ConfigureAwait(false);
        }

        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
    }
}

