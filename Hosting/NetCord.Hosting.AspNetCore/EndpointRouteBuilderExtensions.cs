using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace NetCord.Hosting.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds a route to the <see cref="IEndpointRouteBuilder"/> that will handle Discord interactions.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    public static IEndpointConventionBuilder UseHttpInteractions(this IEndpointRouteBuilder endpoints, string pattern)
    {
        HttpInteractionHandler handler = new(endpoints.ServiceProvider, pattern);

        return endpoints
            .Map(handler.Pattern, handler.HandleRequestAsync)
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
        WebhookEventHandler handler = new(endpoints.ServiceProvider, pattern);

        return endpoints
            .Map(handler.Pattern, handler.HandleRequestAsync)
            .WithMetadata(new HttpMethodMetadata([HttpMethods.Post]));
    }
}
