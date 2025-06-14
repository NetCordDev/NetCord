using System.Reflection;

using NetCord.Rest;
using NetCord.Rest.RateLimits;

namespace NetCord.Hosting.Rest;

public class RestClientOptions : IDiscordOptions
{
    public string? Token { get; set; }

    public string? PublicKey { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.Hostname" />
    public string? Hostname { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.Version" />
    public ApiVersion? Version { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.RequestHandler" />
    public IRestRequestHandler? RequestHandler { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.DefaultRequestProperties" />
    public RestRequestProperties? DefaultRequestProperties { get; set; }

    /// <inheritdoc cref="RestClientConfiguration.RateLimitManager" />
    public IRateLimitManager? RateLimitManager { get; set; }

    internal RestClientConfiguration CreateConfiguration(IServiceProvider services)
    {
        return new()
        {
            Hostname = Hostname,
            Version = Version,
            RequestHandler = RequestHandler ?? CreateDefaultRequestHandler(services),
            DefaultRequestProperties = DefaultRequestProperties,
            RateLimitManager = RateLimitManager,
            Logger = new MicrosoftExtensionsRestLogger(services),
        };
    }

    private static MicrosoftExtensionsRestRequestHandler? CreateDefaultRequestHandler(IServiceProvider services)
    {
        var factoryType = Type.GetType("System.Net.Http.IHttpClientFactory,Microsoft.Extensions.Http");
        if (factoryType is not null)
        {
            var method = factoryType.GetMethod("CreateClient", BindingFlags.Public | BindingFlags.Instance, [typeof(string)]);
            if (method is null || method.ReturnType != typeof(HttpClient))
                return null; // Wrong type

            if (services.GetService(factoryType) is { } factory)
            {
                var client = (HttpClient)method.Invoke(factory, [nameof(RestClient)])!;

                return new(client);
            }
        }

        return null;
    }
}
