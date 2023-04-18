# Parameters

> [!WARNING]
> Parameters are supported only by Slash Commands.

## Optional parameters
To mark parameters as optional, give them a default value, example:
```cs
[SlashCommand("username", "Returns user's username")]
public Task UsernameAsync(User? user = null)
{
    user ??= Context.User;
    return RespondAsync(InteractionCallback.ChannelMessageWithSource(user.Username));
}
```

## Parameter name and description
You can change parameter name and parameter description using @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute, example:
```cs
[SlashCommand("power", "Raises a number to a power")]
public Task PowerAsync([SlashCommandParameter(Name = "base", Description = "The base")] double @base, [SlashCommandParameter(Description = "The power")] double power = 2)
{
    return RespondAsync(InteractionCallback.ChannelMessageWithSource($"Result: {Math.Pow(@base, power)}"));
}
```

## Min and Max Values
You can specify min and max parameter values by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinValue and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxValue properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for numeric types.

## Min and Max Length
You can specify min and max parameter length by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinLength and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxLength properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for text types.

## Choices and Autocomplete
Choices are constants for a given parameter, autocomplete may depend on a text entered by a user.

### Choices
Choices are automatically generated when you set `enum` as a parameter type, you can override choices' names using @NetCord.Services.ApplicationCommands.SlashCommandChoiceAttribute on enum fields, example:
```cs
[SlashCommand("animal", "Sends animal you selected")]
public Task AnimalAsync(Animal animal)
{
    return RespondAsync(InteractionCallback.ChannelMessageWithSource(animal.ToString()));
}

public enum Animal
{
    Dog,
    Cat,
    Fish,
    [SlashCommandChoice(Name = "Guinea Pig")]
    GuineaPig,
}
```
You can also define own choices in Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.ChoicesProvider property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.ChoicesProviderType property.

### Autocomplete
You can turn on autocomplete in Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.AutocompleteProviderType property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.AutocompleteProviderType property. You run it using @NetCord.Services.ApplicationCommands.ApplicationCommandService`2.ExecuteAutocompleteAsync(`1,System.IServiceProvider).