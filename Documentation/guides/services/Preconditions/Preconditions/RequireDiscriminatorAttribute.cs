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

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        // Return a fail result if invalid discriminator
        if (context.User.Discriminator != _discriminator)
            return new(PreconditionResult.Fail($"You need {_discriminator:D4} discriminator to use this."));

        return new(PreconditionResult.Success);
    }
}
