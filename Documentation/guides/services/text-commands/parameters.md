# Parameters

## Variable number of parameters
You can use `params` keyword to accept a variable number of parameters, example:
```cs
[Command("average")]
public Task AverageAsync(params double[] numbers)
{
    return ReplyAsync($"Average: {numbers.Average()}");
}
```

## Optional parameters
To mark parameters as optional, give them a default value, example:
```cs
[Command("power")]
public Task PowerAsync(double @base, double power = 2)
{
    return ReplyAsync($"Result: {Math.Pow(@base, power)}");
}
```

## Remainder
You can set @NetCord.Services.Commands.CommandParameterAttribute.Remainder to true to accept rest of an input as a one parameter, example:
```cs
[Command("reverse")]
public Task ReverseAsync([CommandParameter(Remainder = true)] string toReverse)
{
    var array = toReverse.ToCharArray();
    Array.Reverse(array);
    return ReplyAsync(new string(array));
}
```

## Command overloading
You can overload commands to accept different parameter types, you can specify @NetCord.Services.Commands.CommandAttribute.Priority to set a priority for each command overload, example:
```cs
[Command("multiply", Priority = 0)]
public Task MultiplyAsync(int times, [CommandParameter(Remainder = true)] string text)
{
    return ReplyAsync(string.Concat(Enumerable.Repeat(text, times)));
}

[Command("multiply", Priority = 1)]
public Task MultiplyAsync(int times, BigInteger number)
{
    return ReplyAsync((number * times).ToString());
}
```

> [!NOTE]
> By default, command overloads are selected from the one with the most parameters to that with the fewest parameters.