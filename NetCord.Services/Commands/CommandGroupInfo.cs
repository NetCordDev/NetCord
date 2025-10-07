using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.Commands;

public class CommandGroupInfo<TContext> : ICommandInfo<TContext> where TContext : ICommandContext
{
    public IReadOnlyList<string> Aliases { get; }
    public int Priority { get; }
    public IReadOnlyDictionary<ReadOnlyMemory<char>, IReadOnlyList<ICommandInfo<TContext>>> SubCommands { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    [UnconditionalSuppressMessage("Trimming", "IL2062:Value passed to a method parameter annotated with 'DynamicallyAccessedMembersAttribute' cannot be statically determined and may not meet the attribute's requirements.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "'DynamicallyAccessedMembersAttribute' is inherited for nested types")]
    internal CommandGroupInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type, CommandAttribute attribute, CommandServiceConfiguration<TContext> configuration)
    {
        Aliases = attribute.Aliases.ToArray();

        Priority = attribute.Priority;

        var comparer = configuration.Comparer;

        Dictionary<ReadOnlyMemory<char>, SortedList<ICommandInfo<TContext>>> subCommands = new(comparer);

        var baseType = typeof(BaseCommandModule<TContext>);
        foreach (var nested in type.GetNestedTypes())
        {
            if (!ServiceHelpers.IsModule(baseType, nested))
                continue;

            foreach (var commandAttribute in nested.GetCustomAttributes<CommandAttribute>())
            {
                CommandGroupInfo<TContext> commandGroupInfo = new(nested, commandAttribute, configuration);

                var aliases = commandGroupInfo.Aliases;
                var aliasesCount = aliases.Count;

                for (int i = 0; i < aliasesCount; i++)
                    CommandService<TContext>.DictionaryGetCommandInfoList(subCommands, aliases[i].AsMemory(), configuration).Add(commandGroupInfo);
            }
        }

        foreach (var method in type.GetMethods())
        {
            foreach (var commandAttribute in method.GetCustomAttributes<CommandAttribute>())
            {
                CommandInfo<TContext> commandInfo = new(method, type, commandAttribute, configuration, false);

                var aliases = commandInfo.Aliases;
                var aliasesCount = aliases.Count;

                for (int i = 0; i < aliasesCount; i++)
                    CommandService<TContext>.DictionaryGetCommandInfoList(subCommands, aliases[i].AsMemory(), configuration).Add(commandInfo);
            }
        }

        if (subCommands.Count == 0)
            throw new InvalidOperationException($"No sub commands found in '{type.FullName}'.");

        SubCommands = subCommands.ToFrozenDictionary(p => p.Key, p => (IReadOnlyList<ICommandInfo<TContext>>)p.Value, comparer);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(type);
    }

    internal CommandGroupInfo(CommandGroupBuilder builder, CommandServiceConfiguration<TContext> configuration)
    {
        Aliases = builder.Aliases.ToArray();

        Priority = builder.Priority;

        var comparer = configuration.Comparer;

        Dictionary<ReadOnlyMemory<char>, SortedList<ICommandInfo<TContext>>> subCommands = new(comparer);

        var subCommandGroupBuilders = builder._subCommandGroupBuilders;
        int subCommandGroupCount = subCommandGroupBuilders.Count;

        for (int i = 0; i < subCommandGroupCount; i++)
        {
            var subCommandGroupBuilder = subCommandGroupBuilders[i];
            CommandGroupInfo<TContext> subCommandGroup = new(subCommandGroupBuilder, configuration);

            var aliases = subCommandGroup.Aliases;
            var aliasesCount = aliases.Count;

            for (int j = 0; j < aliasesCount; j++)
                CommandService<TContext>.DictionaryGetCommandInfoList(subCommands, aliases[j].AsMemory(), configuration).Add(subCommandGroup);
        }

        var subCommandBuilders = builder._subCommandBuilders;
        int subCommandCount = subCommandBuilders.Count;

        for (int i = 0; i < subCommandCount; i++)
        {
            var subCommandBuilder = subCommandBuilders[i];
            CommandInfo<TContext> subCommand = new(subCommandBuilder, configuration);

            var aliases = subCommand.Aliases;
            var aliasesCount = aliases.Count;

            for (int j = 0; j < aliasesCount; j++)
                CommandService<TContext>.DictionaryGetCommandInfoList(subCommands, aliases[j].AsMemory(), configuration).Add(subCommand);
        }

        SubCommands = subCommands.ToFrozenDictionary(p => p.Key, p => (IReadOnlyList<ICommandInfo<TContext>>)p.Value, comparer);

        Preconditions = [];
    }

    public async ValueTask<CommandExecutionResult> InvokeAsync(ReadOnlyMemory<char> arguments, TContext context, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return new(preconditionResult, true);

        var separators = configuration.ParameterSeparatorsSearchValues;

        int index = arguments.Span.IndexOfAny(separators);

        var name = index >= 0 ? arguments[..index] : arguments;

        if (!SubCommands.TryGetValue(name, out var commandInfos))
            return new CommandExecutionResult(NotFoundResult.Command, true);

        arguments = index >= 0 ? arguments[(index + 1)..].TrimStart(separators) : default;

        int count = commandInfos.Count;

        CommandExecutionResult lastResult = default;

        for (int i = 0; i < count; i++)
        {
            lastResult = await commandInfos[i].InvokeAsync(arguments, context, configuration, serviceProvider).ConfigureAwait(false);

            if (!lastResult.ContinueNextOverload)
                break;
        }

        return lastResult;
    }
}
