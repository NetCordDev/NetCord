using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services;

public interface IResultResolverProvider<TContext>
{
    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver);
}
