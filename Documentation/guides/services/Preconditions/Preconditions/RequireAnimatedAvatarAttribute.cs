using NetCord.Services;

namespace MyBot;

// We use generics to make our attribute usable for all types of commands and interactions at the same time
public class RequireAnimatedAvatarAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        // Return a fail result when the user has no avatar or it's not animated
        if (context.User.AvatarHash is not string hash || !hash.StartsWith("a_"))
            return new(PreconditionResult.Fail("You need an animated avatar to use this."));

        return new(PreconditionResult.Success);
    }
}
