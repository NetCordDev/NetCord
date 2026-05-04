using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

using static NetCord.Rest.InteractionCallback;

namespace NetCord.Services;

internal static class InteractionResultResolverProviderHelper
{
    public static bool TryGetResolver<TContext>(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver) where TContext : IInteractionContext
    {
        if (type == typeof(Task))
        {
            resolver = static (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            return HandleTaskT(type, out resolver);

        if (type == typeof(void))
        {
            resolver = static (_, _) => default;
            return true;
        }

        if (type.IsAssignableTo(typeof(InteractionCallbackProperties)))
        {
            resolver = static (result, context) =>
            {
                var callback = Unsafe.As<InteractionCallbackProperties>(result!);
                return new(HandleCallbackAsync(context, callback));
            };
            return true;
        }

        if (type == typeof(InteractionMessageProperties))
        {
            resolver = static (result, context) =>
            {
                var message = Unsafe.As<InteractionMessageProperties>(result!);
                return new(HandleCallbackAsync(context, Message(message)));
            };
            return true;
        }

        if (type == typeof(string))
        {
            resolver = static (result, context) =>
            {
                var content = Unsafe.As<string>(result!);
                return new(HandleCallbackAsync(context, Message(content)));
            };
            return true;
        }

        if (type == typeof(ModalProperties))
        {
            resolver = static (result, context) =>
            {
                var modal = Unsafe.As<ModalProperties>(result!);
                return new(HandleCallbackAsync(context, Modal(modal)));
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static bool HandleTaskT<TContext>(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver) where TContext : IInteractionContext
    {
        var genericArgument = type.GetGenericArguments()[0];

        if (genericArgument.IsAssignableTo(typeof(InteractionCallbackProperties)))
            return HandleTaskCallback(genericArgument, out resolver);

        if (genericArgument == typeof(InteractionMessageProperties))
        {
            resolver = static async (result, context) =>
            {
                var message = await Unsafe.As<Task<InteractionMessageProperties>>(result!).ConfigureAwait(false);
                await HandleCallbackAsync(context, Message(message)).ConfigureAwait(false);
            };
            return true;
        }

        if (genericArgument == typeof(string))
        {
            resolver = static async (result, context) =>
            {
                var content = await Unsafe.As<Task<string>>(result!).ConfigureAwait(false);
                await HandleCallbackAsync(context, Message(content)).ConfigureAwait(false);
            };
            return true;
        }

        if (genericArgument == typeof(ModalProperties))
        {
            resolver = static async (result, context) =>
            {
                var modal = await Unsafe.As<Task<ModalProperties>>(result!).ConfigureAwait(false);
                await HandleCallbackAsync(context, Modal(modal)).ConfigureAwait(false);
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static bool HandleTaskCallback<TContext>(Type callbackType, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver) where TContext : IInteractionContext
    {
        if (callbackType.IsGenericType && callbackType.GetGenericTypeDefinition() == typeof(InteractionCallbackProperties<>))
            return HandleTaskCallbackT(callbackType, out resolver);

        if (callbackType == typeof(InteractionCallbackProperties))
        {
            resolver = static async (result, context) =>
            {
                var callback = await Unsafe.As<Task<InteractionCallbackProperties>>(result!).ConfigureAwait(false);
                await HandleCallbackAsync(context, callback).ConfigureAwait(false);
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static bool HandleTaskCallbackT<TContext>(Type callbackType, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver) where TContext : IInteractionContext
    {
        static Task<InteractionCallbackProperties<T>> GetCallbackTask<T>(object? result)
        {
            return Unsafe.As<Task<InteractionCallbackProperties<T>>>(result!);
        }

        var genericArgument = callbackType.GetGenericArguments()[0];

        if (genericArgument == typeof(InteractionMessageProperties))
        {
            resolver = static async (result, context) =>
            {
                var callback = await GetCallbackTask<InteractionMessageProperties>(result).ConfigureAwait(false);
                await HandleCallbackAsync(context, callback).ConfigureAwait(false);
            };
            return true;
        }

        if (genericArgument == typeof(MessageOptions))
        {
            resolver = static async (result, context) =>
            {
                var callback = await GetCallbackTask<MessageOptions>(result).ConfigureAwait(false);
                await HandleCallbackAsync(context, callback).ConfigureAwait(false);
            };
            return true;
        }

        if (genericArgument == typeof(InteractionCallbackChoicesDataProperties))
        {
            resolver = static async (result, context) =>
            {
                var callback = await GetCallbackTask<InteractionCallbackChoicesDataProperties>(result).ConfigureAwait(false);
                await HandleCallbackAsync(context, callback).ConfigureAwait(false);
            };
            return true;
        }

        if (genericArgument == typeof(ModalProperties))
        {
            resolver = static async (result, context) =>
            {
                var callback = await GetCallbackTask<ModalProperties>(result).ConfigureAwait(false);
                await HandleCallbackAsync(context, callback).ConfigureAwait(false);
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static Task<InteractionCallbackResponse?> HandleCallbackAsync<TContext>(TContext context, InteractionCallbackProperties callback) where TContext : IInteractionContext
    {
        return context.Interaction.SendResponseAsync(callback);
    }
}
