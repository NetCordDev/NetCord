using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Rest;

namespace HostingTest;

public sealed class HttpInteractionHandlerTester : IHandlerTester
{
    private static HostApplicationBuilder CreateHttpInteractionHostBuilder(Func<IHttpInteractionHandlerInvoker, IServiceProvider, CancellationToken, ValueTask> invokeAction)
    {
        var builder = Helper.CreateBuilder();

        builder.Services
            .AddDiscordRest()
            .AddHttpInteractionHandlerInvoker()
            .AddHostedService(services => new InvokerBackgroundService<IHttpInteractionHandlerInvoker>(services, invokeAction));

        return builder;
    }

    private static ValueTask InvokeInteractionAsync(IHttpInteractionHandlerInvoker invoker, IServiceProvider services, CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<RestClient>();

        var interaction = Interaction.CreateFromJson(
            new()
            {
                Type = InteractionType.ApplicationCommand,
                Data = new()
                {
                    Type = ApplicationCommandType.ChatInput,
                    Name = "test",
                    Id = 123,
                    Options = [],
                },
                User = new()
                {
                    Id = 1234,
                    Username = "test",
                },
                Channel = new()
                {
                    Id = 12345,
                    Type = ChannelType.DMChannel,
                },
                Entitlements = [],
            },
            null,
            (_, _, _, _, _) => Task.FromResult((InteractionCallbackResponse?)null),
            client);

        return invoker.InvokeAsync(interaction);
    }

    public static IHost CreateClassTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddSingleton(counter)
            .AddHttpInteractionHandler<HttpInteractionHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddSingleton(counter)
            .AddScoped(_ => string.Empty)
            .AddHttpInteractionHandler(lifetime is ServiceLifetime.Singleton ? typeof(RejectingStringHttpInteractionHandler)
                                                                             : typeof(RequiringStringHttpInteractionHandler), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<HttpInteractionHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddScoped(_ => string.Empty)
            .AddHttpInteractionHandler(_ => new HttpInteractionHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddSingleton(counter)
            .AddHttpInteractionHandler<DisposableHttpInteractionHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddSingleton(counter)
            .AddHttpInteractionHandler<AsyncDisposableHttpInteractionHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddSingleton(counter)
            .AddHttpInteractionHandler<DisposableAndAsyncDisposableHttpInteractionHandler>(lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<DisposableHttpInteractionHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<AsyncDisposableHttpInteractionHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<DisposableAndAsyncDisposableHttpInteractionHandler>(_ => new(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableTestHost(DisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<IHttpInteractionHandler>(_ => new DisposableHttpInteractionHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<IHttpInteractionHandler>(_ => new AsyncDisposableHttpInteractionHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateClassFactoryHiddenDisposableAndAsyncDisposableTestHost(AsyncDisposableCounter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler<IHttpInteractionHandler>(_ => new DisposableAndAsyncDisposableHttpInteractionHandler(counter), lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler(() =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateWithParametersTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

        builder.Services
            .AddHttpInteractionHandler((Interaction arg) =>
            {
                counter.HandlerCount++;
            }, lifetime);

        return builder.Build();
    }

    public static IHost CreateDelegateScopedTestHost(Counter counter, ServiceLifetime lifetime)
    {
        var builder = CreateHttpInteractionHostBuilder(InvokeInteractionAsync);

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
            .AddHttpInteractionHandler(handler, lifetime);

        return builder.Build();
    }

    private class HttpInteractionHandler : IHttpInteractionHandler
    {
        private readonly Counter _counter;

        public HttpInteractionHandler(Counter counter)
        {
            _counter = counter;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(Interaction interaction)
        {
            _counter.HandlerCount++;
            return default;
        }
    }

    private class DisposableHttpInteractionHandler(DisposableCounter counter) : HttpInteractionHandler(counter), IDisposable
    {
        public void Dispose()
        {
            counter.DisposeCount++;
        }
    }

    private class AsyncDisposableHttpInteractionHandler(AsyncDisposableCounter counter) : HttpInteractionHandler(counter), IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            counter.DisposeAsyncCount++;
            return default;
        }
    }

    private class DisposableAndAsyncDisposableHttpInteractionHandler(AsyncDisposableCounter counter) : HttpInteractionHandler(counter), IDisposable, IAsyncDisposable
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

    private class RequiringStringHttpInteractionHandler : IHttpInteractionHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RequiringStringHttpInteractionHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(Interaction interaction)
        {
            _ = _services.GetRequiredService<string>();

            _counter.HandlerCount++;
            return default;
        }
    }

    private class RejectingStringHttpInteractionHandler : IHttpInteractionHandler
    {
        private readonly Counter _counter;
        private readonly IServiceProvider _services;

        public RejectingStringHttpInteractionHandler(Counter counter, IServiceProvider services)
        {
            _counter = counter;
            _services = services;

            counter.ConstructorCount++;
        }

        public ValueTask HandleAsync(Interaction interaction)
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
}
