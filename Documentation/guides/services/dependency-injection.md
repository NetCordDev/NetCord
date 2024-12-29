# Dependency Injection

Dependency injection (DI) is a technique that helps make your code more modular and testable by letting you pass services from the outside. It reduces tight coupling between components, making your applications easier to maintain and extend.

## Scopes

With `NetCord.Hosting.Services` scopes are created for each command/interaction and disposed after the command/interaction is **completely** executed by default. Therefore all code relevant to the command/interaction like for example the @NetCord.Hosting.Services.ApplicationCommands.IApplicationCommandResultHandler`1 will be executed within the same scope.

You can control whether to use scopes or not by setting the `UseScopes` property in the options class. For example @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceOptions`2.UseScopes for application commands.

## Minimal APIs

Dependency injection with minimal APIs can seem complicated at first, but it is actually quite simple.

What you need to know is that parameters preceding the context parameter are treated as services and parameters following the context parameter are treated as command/interaction parameters. When the context parameter is not present, all parameters are treated as command/interaction parameters.

You can see an example slash command below, but the same rules apply to all services:
[!code-cs[Program.cs](DependencyInjection/Program.cs#L27-L32)]

## Modules

Dependency injection with modules is like everywhere else. You just inject the services via the constructor. The modules behave as if they were transient services, so they are created for each command/interaction.

You can see an example with text commands below, but the same rules apply to all services:
[!code-cs[DataModule.cs](DependencyInjection/DataModule.cs#l5-L9)]

## Autocomplete Providers

Same for autocomplete providers, you just inject the services via the constructor. They also behave as if they were transient services.

You can see an example autocomplete provider below:
[!code-cs[DataAutocompleteProvider.cs](DependencyInjection/DataAutocompleteProvider.cs#l7-L22)]
