# Aliases

Each command can have multiple aliases, example:
```cs
[Command("average", "mean")]
public Task AverageAsync(params double[] numbers)
{
    return ReplyAsync($"Average: {numbers.Average()}");
}
```