using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services;

internal static class InteractionResultResolverProviderHelper
{
    public static bool TryGetResolver<TContext>(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver) where TContext : IInteractionContext
    {
        if (type == typeof(Task))
        {
            resolver = (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type == typeof(Task<InteractionCallbackProperties<InteractionMessageProperties>>))
        {
            resolver = HandleTaskInteractionCallback<TContext, InteractionMessageProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallbackProperties<MessageOptions>>))
        {
            resolver = HandleTaskInteractionCallback<TContext, MessageOptions>;
            return true;
        }

        if (type == typeof(Task<InteractionCallbackProperties<InteractionCallbackChoicesDataProperties>>))
        {
            resolver = HandleTaskInteractionCallback<TContext, InteractionCallbackChoicesDataProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallbackProperties<ModalProperties>>))
        {
            resolver = HandleTaskInteractionCallback<TContext, ModalProperties>;
            return true;
        }

        if (type == typeof(Task<InteractionCallbackProperties>))
        {
            resolver = async (result, context) =>
            {
                var callback = await Unsafe.As<Task<InteractionCallbackProperties>>(result!).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(Task<InteractionMessageProperties>))
        {
            resolver = async (result, context) =>
            {
                var message = await Unsafe.As<Task<InteractionMessageProperties>>(result!).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(InteractionCallback.Message(message)).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(Task<string>))
        {
            resolver = async (result, context) =>
            {
                var content = await Unsafe.As<Task<string>>(result!).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(InteractionCallback.Message(content)).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(Task<ModalProperties>))
        {
            resolver = async (result, context) =>
            {
                var modal = await Unsafe.As<Task<ModalProperties>>(result!).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal)).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(void))
        {
            resolver = (_, _) => default;
            return true;
        }

        if (type.IsAssignableTo(typeof(InteractionCallbackProperties)))
        {
            resolver = (result, context) =>
            {
                var callback = Unsafe.As<InteractionCallbackProperties>(result!);
                return new(context.Interaction.SendResponseAsync(callback));
            };
            return true;
        }

        if (type == typeof(InteractionMessageProperties))
        {
            resolver = (result, context) =>
            {
                var message = Unsafe.As<InteractionMessageProperties>(result!);
                return new(context.Interaction.SendResponseAsync(InteractionCallback.Message(message)));
            };
            return true;
        }

        if (type == typeof(string))
        {
            resolver = (result, context) =>
            {
                var content = Unsafe.As<string>(result!);
                return new(context.Interaction.SendResponseAsync(InteractionCallback.Message(content)));
            };
            return true;
        }

        if (type == typeof(ModalProperties))
        {
            resolver = (result, context) =>
            {
                var modal = Unsafe.As<ModalProperties>(result!);
                return new(context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal)));
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static async ValueTask HandleTaskInteractionCallback<TContext, T>(object? result, TContext context) where TContext : IInteractionContext
    {
        var callback = await Unsafe.As<Task<InteractionCallbackProperties<T>>>(result!).ConfigureAwait(false);
        await context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);
    }
}
