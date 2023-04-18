# Dependency Injection

To use dependency injection, simply create a constructor with parameters in a module or an autocomplete provider and then pass `IServiceProvider` as the last parameter of `ExecuteAsync` or `ExecuteAutocompleteAsync` method.

## Example Module

```cs
public class ExampleModule : CommandModule<CommandContext>
{
    private readonly string _botName;

    public ExampleModule(string botName)
    {
        _botName = botName;
    }

    [Command("name")]
    public Task NameAsync()
    {
        return ReplyAsync(_botName);
    }
}
```

## Example Autocomplete Provider

```cs
public class ExampleAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private readonly string[] _data;

    public ExampleAutocompleteProvider(string[] data)
    {
        _data = data;
    }

    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var input = option.Value!;
        var result = _data.Where(d => d.Contains(input))
                          .Take(25)
                          .Select(d => new ApplicationCommandOptionChoiceProperties(d, d));

        return Task.FromResult<IEnumerable<ApplicationCommandOptionChoiceProperties>?>(result);
    }
}
```