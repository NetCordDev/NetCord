# Dependency Injection

Dependency injection (DI) is a technique that helps make your code more modular and testable by letting you pass services from the outside. It reduces tight coupling between components, making your applications easier to maintain and extend.

## Minimal APIs

Dependency injection with minimal APIs can seem complicated at first, but it is actually quite simple.

What you need to know is that parameters preceding the context parameter are treated as services and parameters following the context parameter are treated as command/interaction parameters. When the context parameter is not present, all parameters are treated as command/interaction parameters.

You can see an example slash command below, but the same rules apply to all services:
[!code-cs[Program.cs](DependencyInjection/Program.cs#L27-L31)]

## Modules

Dependency injection with modules is like everywhere else. You just inject the services via the constructor.

You can see an example with text commands below, but the same rules apply to all services:
[!code-cs[DataModule.cs](DependencyInjection/DataModule.cs#l5-L9)]

## Autocomplete Providers

Same for autocomplete providers, you just inject the services via the constructor.

You can see an example autocomplete provider below:
[!code-cs[DataAutocompleteProvider.cs](DependencyInjection/DataAutocompleteProvider.cs#l7-L22)]