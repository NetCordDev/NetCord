using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.AspNetCore;

public static class HttpEventHostBuilderExtensions
{
    /// <summary>
    /// Adds an <see cref="IHttpInteractionParser"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IHttpInteractionParser"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseHttpInteractionParser(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddHttpInteractionParser());
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandlerInvoker"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IHttpInteractionHandlerInvoker"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseHttpInteractionHandlerInvoker(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddHttpInteractionHandlerInvoker());
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionProcessor"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IHttpInteractionProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseHttpInteractionProcessor(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddHttpInteractionProcessor());
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventParser"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IWebhookEventParser"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseWebhookEventParser(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddWebhookEventParser());
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandlerInvoker"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IWebhookEventHandlerInvoker"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseWebhookEventHandlerInvoker(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddWebhookEventHandlerInvoker());
        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventProcessor"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="IWebhookEventProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseWebhookEventProcessor(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => services.AddWebhookEventProcessor());
        return builder;
    }
}
