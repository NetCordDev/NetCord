# Parameters

## Variable number of parameters
You can use `params` keyword to accept a variable number of parameters, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L9-L10)]

## Optional parameters
To mark parameters as optional, give them a default value, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L12-L13)]

## Remainder
You can set @NetCord.Services.Commands.CommandParameterAttribute.Remainder to true to accept rest of an input as a one parameter, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L15-L21)]

## Command overloading
You can overload commands to accept different parameter types, you can specify @NetCord.Services.Commands.CommandAttribute.Priority to set a priority for each command overload, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L23-L30)]

> [!NOTE]
> By default, command overloads are selected from the one with the most parameters to that with the fewest parameters.