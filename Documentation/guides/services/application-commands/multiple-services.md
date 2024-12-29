# Multiple Services

> [!NOTE]
> When using hosting, multiple instances of @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 are handled automatically, so no additional configuration is needed.

In some scenarios, you might want to use multiple instances of @NetCord.Services.ApplicationCommands.ApplicationCommandService`1. For instance, you may want to use different contexts for different application commands. In such cases, you can utilize the @NetCord.Services.ApplicationCommands.ApplicationCommandServiceManager, which allows you to send commands from different instances of @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 to Discord simultaneously.

[!code-cs[Program.cs](MultipleServices/Program.cs)]
