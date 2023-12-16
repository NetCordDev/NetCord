# Dependency Injection

To use dependency injection, simply create a constructor with parameters in a module or an autocomplete provider and then pass `IServiceProvider` as the last parameter of `ExecuteAsync` or `ExecuteAutocompleteAsync` method. It is done automatically when using hosting.

## Example Module
[!code-cs[ExampleModule.cs](DependencyInjection/ExampleModule.cs)]

## Example Autocomplete Provider
[!code-cs[ExampleAutocompleteProvider.cs](DependencyInjection/ExampleAutocompleteProvider.cs)]