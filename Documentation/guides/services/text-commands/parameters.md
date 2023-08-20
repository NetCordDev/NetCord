# Parameters

## Variable number of parameters
You can use `params` keyword to accept a variable number of parameters, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L9-L13)]

## Optional parameters
To mark parameters as optional, give them a default value, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L15-L19)]

## Remainder
You can set @NetCord.Services.Commands.CommandParameterAttribute.Remainder to true to accept rest of an input as a one parameter, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L21-L27)]

## Command overloading
You can overload commands to accept different parameter types, you can specify @NetCord.Services.Commands.CommandAttribute.Priority to set a priority for each command overload, example:
[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L29-L39)]

> [!NOTE]
> By default, command overloads are selected from the one with the most parameters to that with the fewest parameters.