using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NetCord.Rest;

namespace NetCord.Services.Commands;

public class CommandResultResolverProvider<TContext> : IResultResolverProvider<TContext> where TContext : ICommandContext
{
    public static CommandResultResolverProvider<TContext> Instance { get; } = new();

    private CommandResultResolverProvider()
    {
    }

    public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
    {
        if (type == typeof(Task))
        {
            resolver = static (result, context) => new(Unsafe.As<Task>(result!));
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var genericArgument = type.GetGenericArguments()[0];

            if (genericArgument == typeof(ReplyMessageProperties))
            {
                resolver = static async (result, context) =>
                {
                    var messageProperties = await Unsafe.As<Task<ReplyMessageProperties>>(result!).ConfigureAwait(false);
                    await HandleReplyAsync(context, messageProperties).ConfigureAwait(false);
                };
                return true;
            }

            if (genericArgument == typeof(MessageProperties))
            {
                resolver = static async (result, context) =>
                {
                    var messageProperties = await Unsafe.As<Task<MessageProperties>>(result!).ConfigureAwait(false);
                    await HandleMessageAsync(context, messageProperties).ConfigureAwait(false);
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
        }

        if (type == typeof(void))
        {
            resolver = static (_, _) => default;
            return true;
        }

        if (type == typeof(ReplyMessageProperties))
        {
            resolver = static (result, context) =>
            {
                var messageProperties = Unsafe.As<ReplyMessageProperties>(result!);
                return new(HandleReplyAsync(context, messageProperties));
            };
            return true;
        }

        if (type == typeof(MessageProperties))
        {
            resolver = static (result, context) =>
            {
                var messageProperties = Unsafe.As<MessageProperties>(result!);
                return new(HandleMessageAsync(context, messageProperties));
            };
            return true;
        }

        if (type == typeof(string))
        {
            resolver = static (result, context) =>
            {
                var message = Unsafe.As<string>(result!);
                return new(HandleContentAsync(context, message));
            };
            return true;
        }

        resolver = null;
        return false;
    }

    private static Task<RestMessage> HandleReplyAsync(TContext context, ReplyMessageProperties messageProperties)
    {
        return context.Message.ReplyAsync(messageProperties);
    }

    private static Task<RestMessage> HandleMessageAsync(TContext context, MessageProperties messageProperties)
    {
        return context.Message.SendAsync(messageProperties);
    }

    private static Task<RestMessage> HandleContentAsync(TContext context, string content)
    {
        return context.Message.ReplyAsync(content);
    }
}
