using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : IComponentInteractionContext
{
    public static ComponentInteractionResultResolverProvider<TContext> Instance { get; } = new();

    private ComponentInteractionResultResolverProvider()
    {
    }

    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
    {
        return InteractionResultResolverProviderHelper.TryGetResolver(type, out resolver);
    }
}
