using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for text commands, application commands and interactions at the same time
public class RequireDiscriminatorAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext
{
    private readonly ushort _discriminator;

    public RequireDiscriminatorAttribute(ushort discriminator)
    {
        _discriminator = discriminator;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        // Throw exception if invalid discriminator
        if (context.User.Discriminator != _discriminator)
            throw new($"You need {_discriminator:D4} discriminator to use this command.");

        return default;
    }
}
