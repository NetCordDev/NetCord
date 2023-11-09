using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.Commands;

public class CommandResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : ICommandContext
{
    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
    {
        if (type == typeof(Task))
        {
            resolver = (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type == typeof(Task<ReplyMessageProperties>))
        {
            resolver = async (result, context) =>
            {
                var messageProperties = await Unsafe.As<Task<ReplyMessageProperties>>(result!).ConfigureAwait(false);
                await context.Message.ReplyAsync(messageProperties).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(Task<MessageProperties>))
        {
            resolver = async (result, context) =>
            {
                var messageProperties = await Unsafe.As<Task<MessageProperties>>(result!).ConfigureAwait(false);
                await context.Message.SendAsync(messageProperties).ConfigureAwait(false);
            };
            return true;
        }

        if (type == typeof(void))
        {
            resolver = (_, _) => default;
            return true;
        }

        if (type == typeof(ReplyMessageProperties))
        {
            resolver = (result, context) =>
            {
                var messageProperties = Unsafe.As<ReplyMessageProperties>(result!);
                return new(context.Message.ReplyAsync(messageProperties));
            };
            return true;
        }

        if (type == typeof(MessageProperties))
        {
            resolver = (result, context) =>
            {
                var messageProperties = Unsafe.As<MessageProperties>(result!);
                return new(context.Message.SendAsync(messageProperties));
            };
            return true;
        }

        resolver = null;
        return false;
    }
}
