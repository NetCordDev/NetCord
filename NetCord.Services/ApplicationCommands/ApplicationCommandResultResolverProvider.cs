using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : IApplicationCommandContext
{
    public static ApplicationCommandResultResolverProvider<TContext> Instance { get; } = new();

    private ApplicationCommandResultResolverProvider()
    {
    }

    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
    {
        return InteractionResultResolverProviderHelper.TryGetResolver(type, out resolver);
    }
}
