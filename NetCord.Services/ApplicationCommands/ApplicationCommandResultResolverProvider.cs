using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

//public class HttpApplicationCommandResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : IApplicationCommandContext
//{
//    private static readonly Task<InteractionCallback?> _nullTask = Task.FromResult<InteractionCallback?>(null);

//    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
//    {
//        if (type == typeof(Task))
//        {
//            resolver = (result, context) => new(Task.FromResult<InteractionCallback?>(null));
//            return true;
//        }

//        if (type == typeof(Task<InteractionCallback<InteractionMessageProperties>>))
//        {
//            resolver = HandleTaskInteractionCallback<InteractionMessageProperties>;
//            return true;
//        }

//        if (type == typeof(Task<InteractionCallback<MessageOptions>>))
//        {
//            resolver = HandleTaskInteractionCallback<MessageOptions>;
//            return true;
//        }

//        if (type == typeof(Task<InteractionCallback<InteractionCallbackChoicesDataProperties>>))
//        {
//            resolver = HandleTaskInteractionCallback<InteractionCallbackChoicesDataProperties>;
//            return true;
//        }

//        if (type == typeof(Task<InteractionCallback<ModalProperties>>))
//        {
//            resolver = HandleTaskInteractionCallback<ModalProperties>;
//            return true;
//        }

//        if (type == typeof(Task<InteractionCallback>))
//        {
//            resolver = (result, context) =>
//            {
//                var callback = Unsafe.As<Task<InteractionCallback?>>(result!);
//                return new(callback);
//            };
//            return true;
//        }

//        if (type == typeof(Task<InteractionMessageProperties>))
//        {
//            resolver = (result, context) =>
//            {
//                var message = Unsafe.As<Task<InteractionMessageProperties>>(result!);
//                return new(message.ContinueWith(task => (InteractionCallback)InteractionCallback.Message(task.Result)));
//            };
//            return true;
//        }

//        if (type == typeof(Task<string>))
//        {
//            resolver = (result, context) =>
//            {
//                var content = Unsafe.As<Task<string>>(result!);
//                return new(content.ContinueWith(task => (InteractionCallback)InteractionCallback.Message(task.Result)));
//            };
//            return true;
//        }

//        if (type == typeof(void))
//        {
//            resolver = (_, _) => new(_nullTask);
//            return true;
//        }

//        if (type.IsAssignableTo(typeof(InteractionCallback)))
//        {
//            resolver = (result, context) =>
//            {
//                var callback = Unsafe.As<InteractionCallback>(result!);
//                return new(Task.FromResult(callback));
//            };
//            return true;
//        }

//        if (type == typeof(InteractionMessageProperties))
//        {
//            resolver = (result, context) =>
//            {
//                var message = Unsafe.As<InteractionMessageProperties>(result!);
//                return new(Task.FromResult(InteractionCallback.Message(message)));
//            };
//            return true;
//        }

//        if (type == typeof(string))
//        {
//            resolver = (result, context) =>
//            {
//                var content = Unsafe.As<string>(result!);
//                return new(Task.FromResult(InteractionCallback.Message(content)));
//            };
//            return true;
//        }

//        resolver = null;
//        return false;

//    }

//    private static ValueTask HandleTaskInteractionCallback<T>(object? result, TContext context)
//    {
//        var callback = Unsafe.As<Task<InteractionCallback<T>?>>(result!);
//        return new(callback.ContinueWith(t => (InteractionCallback?)t.Result));
//    }
//}

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

        resolver = null;
        return false;
    }

    private static async ValueTask HandleTaskInteractionCallback<T>(object? result, TContext context)
    {
        var callback = await Unsafe.As<Task<InteractionCallback<T>>>(result!).ConfigureAwait(false);
        await context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);
    }
}
