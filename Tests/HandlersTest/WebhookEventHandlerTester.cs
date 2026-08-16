using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.JsonModels;
using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace HandlersTest;

public sealed class WebhookEventHandlerTester : ISingleClassMultipleHandlersSupportedHandlerTester
{
    private static HostApplicationBuilder CreateWebhookHostBuilder(Func<IWebhookEventHandlerInvoker, IServiceProvider, CancellationToken, ValueTask> invokeAction)
    {
        var builder = Helper.CreateBuilder();

        builder.Services
            .AddDiscordRest()
            .AddWebhookEventHandlerInvoker()
            .AddHostedService(services => new InvokerBackgroundService<IWebhookEventHandlerInvoker>(services, invokeAction));

        return builder;
    }

    private static ValueTask InvokeApplicationDeauthorizedAsync(IWebhookEventHandlerInvoker invoker, IServiceProvider services, CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<RestClient>();

        var args = WebhookEventArgs.CreateFromJson(new JsonWebhookEventArgs
        {
            Type = WebhookEventType.Event,
            Event = new()
            {
                Type = "APPLICATION_DEAUTHORIZED",
                Data = JsonSerializer.SerializeToElement(new JsonApplicationDeauthorizedWebhookEventData()
                {
                    User = new()
                    {
                        Id = 1234,
                        Username = "test",
                    },
                })
            }
        }, client);

        return invoker.InvokeAsync(args);
    }

    private static ValueTask InvokeApplicationAuthorizedAsync(IWebhookEventHandlerInvoker invoker, IServiceProvider services, CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        var client = services.GetRequiredService<RestClient>();

        var authorizedArgs = WebhookEventArgs.CreateFromJson(new JsonWebhookEventArgs
        {
            Type = WebhookEventType.Event,
            Event = new()
            {
                Type = "APPLICATION_AUTHORIZED",
                Data = JsonSerializer.SerializeToElement(new JsonApplicationAuthorizedWebhookEventData()
                {
                    User = new()
                    {
                        Id = 1234,
                        Username = "test",
                    },
                    Scopes = [],
                }),
            }
        }, client);

        return invoker.InvokeAsync(authorizedArgs);
    }

    public static IHost CreateClassTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddSingleton(counter)
            .AddWebhookHandler<ApplicationDeauthorizedWebhookHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddSingleton(counter)
            .AddScoped(_ => string.Empty)
            .AddWebhookHandler(lifetime is ServiceLifetime.Singleton ? typeof(RejectingStringApplicationDeauthorizedWebhookHandler)
                                                                     : typeof(RequiringStringApplicationDeauthorizedWebhookHandler), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<ApplicationDeauthorizedWebhookHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddWebhookHandler(_ => new ApplicationDeauthorizedWebhookHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddSingleton(counter)
            .AddWebhookHandler<DisposableApplicationDeauthorizedWebhookHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddSingleton(counter)
            .AddWebhookHandler<AsyncDisposableApplicationDeauthorizedWebhookHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddSingleton(counter)
            .AddWebhookHandler<DisposableAndAsyncDisposableApplicationDeauthorizedWebhookHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<DisposableApplicationDeauthorizedWebhookHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<AsyncDisposableApplicationDeauthorizedWebhookHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<DisposableAndAsyncDisposableApplicationDeauthorizedWebhookHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<IApplicationDeauthorizedWebhookHandler>(_ => new DisposableApplicationDeauthorizedWebhookHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<IApplicationDeauthorizedWebhookHandler>(_ => new AsyncDisposableApplicationDeauthorizedWebhookHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler<IApplicationDeauthorizedWebhookHandler>(_ => new DisposableAndAsyncDisposableApplicationDeauthorizedWebhookHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler(WebhookEvent.ApplicationDeauthorized, () =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateWithParametersTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        builder.Services
            .AddWebhookHandler(WebhookEvent.ApplicationDeauthorized, (ApplicationDeauthorizedWebhookEventArgs arg) =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateWebhookHostBuilder(InvokeApplicationDeauthorizedAsync);

        Action<IServiceProvider> handler;

        if (lifetime is ServiceLifetime.Singleton)
            handler = services =>
            {
                try
                {
                    _ = services.GetRequiredService<string>();
                }
                catch (InvalidOperationException)
                {
                    counter.HandlerCount++;
                }
            };
        else
            handler = services =>
            {
                _ = services.GetRequiredService<string>();
                counter.HandlerCount++;
            };

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddWebhookHandler(WebhookEvent.ApplicationDeauthorized, handler, lifetime);

        return builder.Build();
    }

    public static IHost CreateClassSingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime)
    {
        int turn = 0;

        var builder = CreateWebhookHostBuilder((invoker, services, cancellationToken) =>
        {
            return turn++ % 2 is 0
                ? InvokeApplicationDeauthorizedAsync(invoker, services, cancellationToken)
                : InvokeApplicationAuthorizedAsync(invoker, services, cancellationToken);
        });

        builder.Services
            .AddSingleton(counter1)
            .AddSingleton(counter2)
            .AddWebhookHandler<ApplicationDeauthorizedAndAuthorizedWebhookHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactorySingleMultipleHandlersTestHost(Counter counter1, Counter counter2, ServiceLifetime lifetime)
    {
        int turn = 0;

        var builder = CreateWebhookHostBuilder((invoker, services, cancellationToken) =>
        {
            return turn++ % 2 is 0
                ? InvokeApplicationDeauthorizedAsync(invoker, services, cancellationToken)
                : InvokeApplicationAuthorizedAsync(invoker, services, cancellationToken);
        });

        builder.Services
            .AddWebhookHandler<ApplicationDeauthorizedAndAuthorizedWebhookHandler>(_ => new([counter1, counter2]), lifetime);

        return builder.Build();
    }

    private class ApplicationDeauthorizedWebhookHandler : IApplicationDeauthorizedWebhookHandler
    {
        private readonly Counter _counter;

        public ApplicationDeauthorizedWebhookHandler(Counter counter)
        {
            _counter = counter;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs arg)
        {
            _counter.HandlerCount++;
            return default;
        }
    }

    private class DisposableApplicationDeauthorizedWebhookHandler(DisposableCounter counter) : ApplicationDeauthorizedWebhookHandler(counter), IDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }
    }

    private class AsyncDisposableApplicationDeauthorizedWebhookHandler(AsyncDisposableCounter counter) : ApplicationDeauthorizedWebhookHandler(counter), IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;
            return default;
        }
    }

    private class DisposableAndAsyncDisposableApplicationDeauthorizedWebhookHandler(AsyncDisposableCounter counter) : ApplicationDeauthorizedWebhookHandler(counter), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }

        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;
            return default;
        }
    }

    private class ApplicationDeauthorizedAndAuthorizedWebhookHandler : IApplicationDeauthorizedWebhookHandler, IApplicationAuthorizedWebhookHandler
    {
        private readonly Counter _deauthorizedCounter;
        private readonly Counter _authorizedCounter;

        public ApplicationDeauthorizedAndAuthorizedWebhookHandler(IEnumerable<Counter> counters)
        {
            (_deauthorizedCounter, _authorizedCounter) = Helper.ExtractCounters(counters);

            _deauthorizedCounter.ConstructorCount++;
            _authorizedCounter.ConstructorCount++;
        }

        public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs arg)
        {
            _deauthorizedCounter.HandlerCount++;
            return default;
        }

        public ValueTask HandleAsync(ApplicationAuthorizedWebhookEventArgs arg)
        {
            _authorizedCounter.HandlerCount++;
            return default;
        }
    }

    private class RequiringStringApplicationDeauthorizedWebhookHandler : IApplicationDeauthorizedWebhookHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RequiringStringApplicationDeauthorizedWebhookHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs arg)
        {
            _ = _services.GetRequiredService<string>();

            _counter.HandlerCount++;
            return default;
        }
    }

    private class RejectingStringApplicationDeauthorizedWebhookHandler : IApplicationDeauthorizedWebhookHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RejectingStringApplicationDeauthorizedWebhookHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(ApplicationDeauthorizedWebhookEventArgs arg)
        {
            try
            {
                _ = _services.GetRequiredService<string>();
            }
            catch (InvalidOperationException)
            {
                _counter.HandlerCount++;
            }

            return default;
        }
    }

    private class JsonApplicationDeauthorizedWebhookEventData
    {
        [JsonPropertyName("user")]
        public required JsonUser User { get; set; }
    }

    private class JsonApplicationAuthorizedWebhookEventData
    {
        [JsonPropertyName("integration_type")]
        public ApplicationIntegrationType? IntegrationType { get; set; }

        [JsonPropertyName("user")]
        public required JsonUser User { get; set; }

        [JsonPropertyName("scopes")]
        public required string[] Scopes { get; set; }

        [JsonPropertyName("guild")]
        public JsonGuild? Guild { get; set; }
    }
}
