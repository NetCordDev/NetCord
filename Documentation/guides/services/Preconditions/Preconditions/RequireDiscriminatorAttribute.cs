using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for text commands, application commands and interactions at the same time
public class RequireDiscriminatorAttribute<TContext>(ushort discriminator) : PreconditionAttribute<TContext> where TContext : IUserContext
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        // Return a fail result if invalid discriminator
        if (context.User.Discriminator != discriminator)
            return new(PreconditionResult.Fail($"You need {discriminator:D4} discriminator to use this."));

        return new(PreconditionResult.Success);
    }
}
