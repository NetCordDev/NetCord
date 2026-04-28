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
            resolver = static (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var genericArgument = type.GetGenericArguments()[0];

            if (genericArgument.IsAssignableTo(typeof(InteractionCallbackProperties)))
            {
                resolver = static async (result, context) =>
                {
                    var callback = await Unsafe.As<Task<InteractionCallbackProperties>>(result!).ConfigureAwait(false);
                    await HandleCallbackAsync(context, callback).ConfigureAwait(false);
                };
                return true;
            }

            if (genericArgument == typeof(InteractionMessageProperties))
            {
                resolver = static async (result, context) =>
                {
                    var message = await Unsafe.As<Task<InteractionMessageProperties>>(result!).ConfigureAwait(false);
                    await HandleMessageAsync(context, message).ConfigureAwait(false);
                };
                return true;
            }

            if (genericArgument == typeof(string))
            {
                resolver = static async (result, context) =>
                {
                    var content = await Unsafe.As<Task<string>>(result!).ConfigureAwait(false);
                    await HandleContentAsync(context, content).ConfigureAwait(false);
                };
                return true;
            }

            if (genericArgument == typeof(ModalProperties))
            {
                resolver = static async (result, context) =>
                {
                    var modal = await Unsafe.As<Task<ModalProperties>>(result!).ConfigureAwait(false);
                    await HandleModalAsync(context, modal).ConfigureAwait(false);
                };
                return true;
            }
        }

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
                return new(HandleMessageAsync(context, message));
            };
            return true;
        }

        if (type == typeof(string))
        {
            resolver = static (result, context) =>
            {
                var content = Unsafe.As<string>(result!);
                return new(HandleContentAsync(context, content));
            };
            return true;
        }

        if (type == typeof(ModalProperties))
        {
            resolver = static (result, context) =>
            {
                var modal = Unsafe.As<ModalProperties>(result!);
                return new(HandleModalAsync(context, modal));
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

    private static Task<InteractionCallbackResponse?> HandleMessageAsync<TContext>(TContext context, InteractionMessageProperties message) where TContext : IInteractionContext
    {
        return context.Interaction.SendResponseAsync(InteractionCallback.Message(message));
    }

    private static Task<InteractionCallbackResponse?> HandleContentAsync<TContext>(TContext context, string content) where TContext : IInteractionContext
    {
        return context.Interaction.SendResponseAsync(InteractionCallback.Message(content));
    }

    private static Task<InteractionCallbackResponse?> HandleModalAsync<TContext>(TContext context, ModalProperties modal) where TContext : IInteractionContext
    {
        return context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal));
    }
}
