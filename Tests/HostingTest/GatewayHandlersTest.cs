namespace HostingTest;

[TestClass]
public sealed class GatewayHandlersTests(TestContext context) : HandlersTests<GatewayHandlersTester>(context);

// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
//
// using NetCord;
// using NetCord.Gateway;
// using NetCord.Gateway.Compression;
// using NetCord.Gateway.WebSockets;
// using NetCord.Hosting.Gateway;
//
// namespace HostingTest;
//
// [TestClass]
// public class GatewayHandlersTest(TestContext testContext) : GatewayHandlersTestBase
// {
//     private static HostApplicationBuilder CreateBuilder(IWebSocketConnectionProvider webSocketConnectionProvider)
//     {
//         var builder = Helper.CreateBuilder();
//
//         builder.Services
//             .AddDiscordGateway(o =>
//             {
//                 o.WebSocketConnectionProvider = webSocketConnectionProvider;
//                 o.Compression = new UncompressedGatewayCompression();
//                 o.Token = "NO.T.A.REAL.TOKEN";
//             });
//
//         return builder;
//     }
//
//     private class RateLimitedGatewayHandler : IRateLimitedGatewayHandler
//     {
//         private readonly Counter _counter;
//
//         public RateLimitedGatewayHandler(Counter counter)
//         {
//             _counter = counter;
//
//             counter.ConstructorCount++;
//         }
//
//         public ValueTask HandleAsync(RateLimitedEventArgs arg)
//         {
//             _counter.HandlerCount++;
//
//             return default;
//         }
//     }
//
//     private class DisposableRateLimitedGatewayHandler(DisposableCounter counter) : RateLimitedGatewayHandler(counter), IDisposable
//     {
//         public void Dispose()
//         {
//             counter.DisposeCount++;
//         }
//     }
//
//     private class AsyncDisposableRateLimitedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedGatewayHandler(counter), IAsyncDisposable
//     {
//         public ValueTask DisposeAsync()
//         {
//             counter.DisposeAsyncCount++;
//
//             return default;
//         }
//     }
//
//     private class DisposableAndAsyncDisposableRateLimitedGatewayHandler(AsyncDisposableCounter counter) : RateLimitedGatewayHandler(counter), IDisposable, IAsyncDisposable
//     {
//         public void Dispose()
//         {
//             counter.DisposeCount++;
//         }
//
//         public ValueTask DisposeAsync()
//         {
//             counter.DisposeAsyncCount++;
//
//             return default;
//         }
//     }
//
//     private class RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler : IRateLimitedGatewayHandler, IApplicationCommandPermissionsUpdateGatewayHandler
//     {
//         private readonly Counter _rateLimitedCounter;
//         private readonly Counter _applicationCommandPermissionsUpdateCounter;
//
//         public RateLimitedAndApplicationCommandPermissionsUpdateGatewayHandler(Counter rateLimitedCounter, Counter applicationCommandPermissionsUpdateCounter)
//         {
//             _rateLimitedCounter = rateLimitedCounter;
//             _applicationCommandPermissionsUpdateCounter = applicationCommandPermissionsUpdateCounter;
//
//             rateLimitedCounter.ConstructorCount++;
//             applicationCommandPermissionsUpdateCounter.ConstructorCount++;
//         }
//
//         public ValueTask HandleAsync(RateLimitedEventArgs arg)
//         {
//             _rateLimitedCounter.HandlerCount++;
//
//             return default;
//         }
//
//         public ValueTask HandleAsync(ApplicationCommandPermission arg)
//         {
//             _applicationCommandPermissionsUpdateCounter.HandlerCount++;
//
//             return default;
//         }
//     }
//
//     private class RequiringStringRateLimitedGatewayHandler : IRateLimitedGatewayHandler
//     {
//         private readonly Counter _counter;
//         private readonly IServiceProvider _services;
//
//         public RequiringStringRateLimitedGatewayHandler(Counter counter, IServiceProvider services)
//         {
//             _counter = counter;
//             _services = services;
//
//             counter.ConstructorCount++;
//         }
//
//         public ValueTask HandleAsync(RateLimitedEventArgs arg)
//         {
//             _ = _services.GetRequiredService<string>();
//
//             _counter.HandlerCount++;
//
//             return default;
//         }
//     }
//
//     private class RejectingStringRateLimitedGatewayHandler : IRateLimitedGatewayHandler
//     {
//         private readonly Counter _counter;
//         private readonly IServiceProvider _services;
//
//         public RejectingStringRateLimitedGatewayHandler(Counter counter, IServiceProvider services)
//         {
//             _counter = counter;
//             _services = services;
//
//             counter.ConstructorCount++;
//         }
//
//         public ValueTask HandleAsync(RateLimitedEventArgs arg)
//         {
//             try
//             {
//                 _ = _services.GetRequiredService<string>();
//             }
//             catch (InvalidOperationException)
//             {
//                 _counter.HandlerCount++;
//             }
//
//             return default;
//         }
//     }
// }
