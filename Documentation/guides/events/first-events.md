# First Events

## [.NET Generic Host](#tab/generic-host)

The preferred way to receive events with the .NET Generic Host is by implementing @NetCord.Hosting.Gateway.IGatewayEventHandler or @NetCord.Hosting.Gateway.IGatewayEventHandler`1.

First, use @NetCord.Hosting.Gateway.GatewayEventHandlerServiceCollectionExtensions.AddGatewayEventHandlers(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Reflection.Assembly) to add all event handlers in an assembly. You also need to call @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the handlers to the client.
[!code-cs[Program.cs](FirstEventsHosting/Program.cs?highlight=17,20)]

> [!NOTE]
> All gateway event handlers require @NetCord.Hosting.Gateway.GatewayEventAttribute that specifies the event to bind to. The event argument must match the type of the handler, otherwise an exception will be thrown.

### MessageCreate Event
Now it's time to implement your MessageCreate event handler!
[!code-cs[Program.cs](FirstEventsHosting/MessageCreateHandler.cs)]

When you run this code, when someone sends a message, the message will be printed on a console!

### MessageReactionAdd Event
We will also implement a MessageReactionAdd event handler!
[!code-cs[Program.cs](FirstEventsHosting/MessageReactionAddHandler.cs)]

When you run this code, when someone reacts to a message, the bot will notify everyone about it!

### Other Events
Other events work similar to these. You can play with them if you want!

> [!NOTE]
> When using @NetCord.Gateway.ShardedGatewayClient, you need to implement @NetCord.Hosting.Gateway.IShardedGatewayEventHandler or @NetCord.Hosting.Gateway.IShardedGatewayEventHandler`1 instead. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerServiceCollectionExtensions.AddShardedGatewayEventHandlers* to add event handlers and @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseShardedGatewayEventHandlers* to bind them instead. See @sharding?text=Sharding for more information.

## [Bare Bones](#tab/bare-bones)

### MessageCreate Event
To listen to the event, add the following lines before `client.StartAsync()`!
[!code-cs[Program.cs](FirstEvents/Program.cs#L13-L17)]

When you run this code, when someone sends a message, the message will be printed on a console!

### MessageReactionAdd Event
To listen to the event, add the following lines to your code.
[!code-cs[Program.cs](FirstEvents/Program.cs#L19-L22)]

When you run this code, when someone reacts to a message, the bot will notify everyone about it!

### Other Events
Other events work similar to these. You can play with them if you want!
