# Arguments

## Optional arguments
To mark an argument as optional, give it a default value, example:
```cs
[Command("power")]
public Task PowerAsync(double @base, double power = 2)
{
    return ReplyAsync($"Result: {Math.Pow(@base, power)}");
}
```

## Variable number of arguments
You can use `params` keyword to accept a variable number of arguments, example:
```cs
[Command("average")]
public Task AverageAsync(params double[] numbers)
{
    return ReplyAsync($"Average: {numbers.Average()}");
}
```

## Remainder
You can use `Remainder` attribute to accept rest of input as one argument, example:
```cs
[Command("reverse")]
public Task ReverseAsync([Remainder] string toReverse)
{
    var array = toReverse.ToCharArray();
    Array.Reverse(array);
    return ReplyAsync(new string(array));
}
```

## Command overloading
You can overload commands to accept different argument types, you can specify `Priority` to set a priority for each command overload, example:
```cs
[Command("multiply", Priority = 0)]
public Task MultiplyAsync(int times, [] string text)
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
> By default, command overloads are selected from the one with the most arguments to that with the fewest arguments.