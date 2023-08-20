# Parameters

> [!WARNING]
> Parameters are supported only by Slash Commands.

## Optional parameters
To mark parameters as optional, give them a default value, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L9-L14)]

## Parameter name and description
You can change parameter name and parameter description using @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L16-L20)]

## Min and Max Values
You can specify min and max parameter values by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinValue and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxValue properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for numeric types.

## Min and Max Length
You can specify min and max parameter length by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinLength and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxLength properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for text types.

## Choices and Autocomplete
Choices are constants for a given parameter, autocomplete may depend on a text entered by a user.

### Choices
Choices are automatically generated when you set `enum` as a parameter type, you can override choices' names using @NetCord.Services.ApplicationCommands.SlashCommandChoiceAttribute on enum fields, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L22-L35)]

You can also define own choices in Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.ChoicesProvider property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.ChoicesProviderType property.

### Autocomplete
You can turn on autocomplete in Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.AutocompleteProviderType property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.AutocompleteProviderType property. You run it using @NetCord.Services.ApplicationCommands.ApplicationCommandService`2.ExecuteAutocompleteAsync(`1,System.IServiceProvider).