using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.Commands;

#pragma warning disable IDE0032 // Use auto property

public partial class CommandService<TContext>(CommandServiceConfiguration<TContext>? configuration = null) : ICommandService where TContext : ICommandContext
{
    private readonly CommandServiceConfiguration<TContext> _configuration = configuration ??= CommandServiceConfiguration<TContext>.Default;
    private readonly Dictionary<ReadOnlyMemory<char>, SortedList<ICommandInfo<TContext>>> _commands = new(configuration.Comparer);

    public CommandServiceConfiguration<TContext> Configuration => _configuration;

    public IReadOnlyDictionary<ReadOnlyMemory<char>, IReadOnlyList<ICommandInfo<TContext>>> GetCommands()
        => new Dictionary<ReadOnlyMemory<char>, IReadOnlyList<ICommandInfo<TContext>>>(_commands.Select(c => new KeyValuePair<ReadOnlyMemory<char>, IReadOnlyList<ICommandInfo<TContext>>>(c.Key, [.. c.Value])));

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        foreach (var type in ServiceHelpers.GetTopLevelModules(typeof(BaseCommandModule<TContext>), assembly))
            AddModuleCore(type);
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        if (!ServiceHelpers.IsTopLevelModule(typeof(BaseCommandModule<TContext>), type))
            throw new InvalidOperationException($"Modules cannot be abstract or nested, and must inherit from '{typeof(BaseCommandModule<TContext>)}'.");

        AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicNestedTypes)] Type type)
    {
        var configuration = _configuration;

        bool group = false;

        foreach (var moduleCommandAttribute in type.GetCustomAttributes<CommandAttribute>())
        {
            CommandGroupInfo<TContext> commandGroupInfo = new(type, moduleCommandAttribute, configuration);
            AddCommandInfo(moduleCommandAttribute.Aliases, commandGroupInfo);

            group = true;
        }

        if (group)
            return;

        foreach (var method in type.GetMethods())
        {
            foreach (var commandAttribute in method.GetCustomAttributes<CommandAttribute>())
            {
                CommandInfo<TContext> commandInfo = new(method, type, commandAttribute, configuration, true);
                AddCommandInfo(commandAttribute.Aliases, commandInfo);
            }
        }
    }

    public void AddCommand(CommandBuilder builder)
    {
        CommandInfo<TContext> info = new(builder, _configuration);
        AddCommandInfo(builder.Aliases, info);
    }

    public void AddCommandGroup(CommandGroupBuilder builder)
    {
        CommandGroupInfo<TContext> info = new(builder, _configuration);
        AddCommandInfo(builder.Aliases, info);
    }

    private void AddCommandInfo(IEnumerable<string> aliases, ICommandInfo<TContext> commandInfo)
    {
        var commands = _commands;
        var configuration = _configuration;

        foreach (var alias in aliases)
            DictionaryGetCommandInfoList(commands, alias.AsMemory(), configuration).Add(commandInfo);
    }

    internal static SortedList<ICommandInfo<TContext>> DictionaryGetCommandInfoList(Dictionary<ReadOnlyMemory<char>, SortedList<ICommandInfo<TContext>>> dictionary, ReadOnlyMemory<char> alias, CommandServiceConfiguration<TContext> configuration)
    {
        if (alias.Span.ContainsAny(configuration.ParameterSeparatorsSearchValues))
            throw new InvalidOperationException($"Alias '{alias}' contains characters from '{nameof(configuration.ParameterSeparators)}', which are not allowed.");

        if (!dictionary.TryGetValue(alias, out var list))
        {
            list = new(CommandService<TContext>.CompareCommandInfos);
            dictionary.Add(alias, list);
        }

        return list;
    }

    internal static int CompareCommandInfos(ICommandInfo<TContext> x, ICommandInfo<TContext> y)
    {
        if (y.Priority.CompareTo(x.Priority) is not 0 and int priorityResult)
            return priorityResult;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        var yIsCommand = y is CommandInfo<TContext> yCommand;

        var xIsCommand = x is CommandInfo<TContext> xCommand;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        if (yIsCommand && xIsCommand)
        {
            Unsafe.SkipInit(out yCommand);
            Unsafe.SkipInit(out xCommand);

            return yCommand.Parameters.Count.CompareTo(xCommand.Parameters.Count);
        }

        return xIsCommand.CompareTo(yIsCommand);
    }

    public async ValueTask<IExecutionResult> ExecuteAsync(int prefixLength, TContext context, IServiceProvider? serviceProvider = null)
    {
        try
        {
            return await ExecuteAsyncCore(prefixLength, context, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExecutionExceptionResult(ex);
        }
    }

    private async ValueTask<IExecutionResult> ExecuteAsyncCore(int prefixLength, TContext context, IServiceProvider? serviceProvider)
    {
        var command = context.Message.Content.AsMemory(prefixLength);

        var separators = _configuration.ParameterSeparatorsSearchValues;

        var index = command.Span.IndexOfAny(separators);

        var name = index >= 0 ? command[..index] : command;

        if (!_commands.TryGetValue(name, out var commandInfos))
            return NotFoundResult.Command;

        command = index >= 0 ? command[(index + 1)..].TrimStart(separators) : default;

        int count = commandInfos.Count;

        IExecutionResult lastResult = null!;

        for (int i = 0; i < count; i++)
        {
            var result = await commandInfos[i].InvokeAsync(command, context, _configuration, serviceProvider).ConfigureAwait(false);

            lastResult = result.ExecutionResult;

            if (!result.ContinueNextOverload)
                break;
        }

        return lastResult;
    }
}
