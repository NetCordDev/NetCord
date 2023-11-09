using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : IApplicationCommandContext
{
    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
    {
        if (type == typeof(Task))
        {
            resolver = (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type == typeof(Task<InteractionCallback<InteractionMessageProperties>>))
        {
            resolver = HandleTaskInteractionCallback<InteractionMessageProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallback<MessageOptions>>))
        {
            resolver = HandleTaskInteractionCallback<MessageOptions>;
            return true;
        }

        if (type == typeof(Task<InteractionCallback<InteractionCallbackChoicesDataProperties>>))
        {
            resolver = HandleTaskInteractionCallback<InteractionCallbackChoicesDataProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallback<ModalProperties>>))
        {
            resolver = HandleTaskInteractionCallback<ModalProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallback>))
        {
            resolver = async (result, context) =>
            {
                var callback = await Unsafe.As<Task<InteractionCallback>>(result!).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(void))
        {
            resolver = (_, _) => default;
            return true;
        }

        if (type.IsAssignableTo(typeof(InteractionCallback)))
        {
            resolver = (result, context) =>
            {
                var callback = Unsafe.As<InteractionCallback>(result!);
                return new(context.Interaction.SendResponseAsync(callback));
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static async ValueTask HandleTaskInteractionCallback<T>(object? result, TContext context)
    {
        var callback = await Unsafe.As<Task<InteractionCallback<T>>>(result!).ConfigureAwait(false);
        await context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);
    }
}
