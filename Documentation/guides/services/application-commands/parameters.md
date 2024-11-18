# Parameters

## Slash Commands

Slash commands support up to 25 parameters.

### Optional parameters

To mark parameters as optional, give them a default value, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L8-L13)]

### Parameter name and description

You can change parameter name and parameter description using @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L15-L21)]

### Min and Max Values

You can specify min and max parameter values by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinValue and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxValue properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for numeric types.

### Min and Max Length

You can specify min and max parameter length by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MinLength and @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.MaxLength properties in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute. It's only possible for text types.

### Choices and Autocomplete

Choices are constants for a given parameter, autocomplete may depend on a text entered by a user.

#### Choices

Choices are automatically generated when you set `enum` as a parameter type, you can override choices' names using @NetCord.Services.ApplicationCommands.SlashCommandChoiceAttribute on enum fields, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L23-L24)]
[!code-cs[Animal.cs](Parameters/Animal.cs#l5-L12)]

You can also define own choices in the Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.ChoicesProvider property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.ChoicesProviderType property.

#### Autocomplete

You can turn on autocomplete in Type Reader by overriding @NetCord.Services.ApplicationCommands.SlashCommandTypeReader`1.AutocompleteProviderType property or in @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute by setting @NetCord.Services.ApplicationCommands.SlashCommandParameterAttribute.AutocompleteProviderType property. You run it using @NetCord.Services.ApplicationCommands.ApplicationCommandService`2.ExecuteAutocompleteAsync(`1,System.IServiceProvider).

## User Commands

User commands only support a single parameter of type @NetCord.User. It's is not required, but allows you to access the target user of the user command easily.

## Message Commands

Message commands only support a single parameter of type @NetCord.Rest.RestMessage. It's is not required, but allows you to access the target message of the message command easily.
