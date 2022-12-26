# Preconditions

Preconditions determine whether a command or interaction can be invoked. They are represented by attributes. They can be applied on Modules, Commands and Parameters.

## Built-in Precondition Attributes

> [!NOTE]
> The attributes are generic. Generic attributes are supported since C# 11.

- @NetCord.Services.InteractionRequireBotChannelPermissionsAttribute`1
- @NetCord.Services.InteractionRequireUserChannelPermissionsAttribute`1
- @NetCord.Services.RequireUserPermissionsAttribute`1
- @NetCord.Services.RequireBotPermissionsAttribute`1
- @NetCord.Services.RequireContextAttribute`1
- @NetCord.Services.RequireNsfwAttribute`1

## Creating a Custom Precondition Attribute

```cs
using NetCord.Services;

namespace MyBot;

public class RequireDiscriminatorAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IContext, IUserContext // We use generics to make our attribute usable in text commands, application commands and interactions at the same time
{
    private readonly ushort _discriminator;
    
    public RequireDiscriminatorAttribute(ushort discriminator)
    {
        _discriminator = discriminator;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context)
    {
        // Throw exception if invalid discriminator
        if (context.User.Discriminator != _discriminator)
            throw new($"You need {_discriminator:D4} discriminator to use this command.");

        return default;
    }
}
```

### Example usage

```cs
[RequireDiscriminator<CommandContext>(1234)]
public class FirstModule : CommandModule<CommandContext>
{
    // All commands here will require 1234 discriminator
}
```

## Creating a Custom Parameter Precondition Attribute

```cs
using NetCord.Services;

namespace MyBot;

public class MustContainAttribute<TContext> : ParameterPreconditionAttribute<TContext> where TContext : IContext // We use generics to make our attribute usable in text commands, application commands and interactions at the same time
{
    private readonly string _value;

    public MustContainAttribute(string value)
    {
        _value = value;
    }

    public override ValueTask EnsureCanExecuteAsync(object? value, TContext context)
    {
        // Throw exception if does not contain
        if (!((string)value!).Contains(_value, StringComparison.InvariantCultureIgnoreCase))
            throw new($"The parameter must contain '{_value}'.");

        return default;
    }
}
```

### Example usage

```cs
public class FirstModule : CommandModule<CommandContext>
{
    [Command("hello")]
    public Task HelloAsync([Remainder][MustContain<CommandContext>("hello")] string text)
    {
        return ReplyAsync(text);
    }
}
```