using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class HttpEventEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds a route to the <see cref="IEndpointRouteBuilder"/> that will handle Discord interactions.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder UseHttpInteractions(this IEndpointRouteBuilder endpoints, string pattern)
    {
        var parsedPattern = RoutePatternHelper.ParseLiteral(pattern);

        var processor = endpoints.ServiceProvider.GetService<IHttpInteractionProcessor>()
            ?? new HttpInteractionProcessor(endpoints.ServiceProvider);

        return endpoints
            .Map(parsedPattern, processor.ProcessAsync)
            .WithMetadata(new HttpMethodMetadata([HttpMethods.Post]));
    }

    /// <summary>
    /// Adds a route to the <see cref="IEndpointRouteBuilder"/> that will handle Discord webhook events.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder UseWebhookEvents(this IEndpointRouteBuilder endpoints, string pattern)
    {
        var parsedPattern = RoutePatternHelper.ParseLiteral(pattern);

        var processor = endpoints.ServiceProvider.GetService<IWebhookEventProcessor>()
            ?? new WebhookEventProcessor(endpoints.ServiceProvider);

        return endpoints
            .Map(parsedPattern, processor.ProcessAsync)
            .WithMetadata(new HttpMethodMetadata([HttpMethods.Post]));
    }
}
