using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.Commands;

public class CommandInfo<TContext> : ICommandInfo<TContext> where TContext : ICommandContext
{
    public IReadOnlyList<string> Aliases { get; }
    public int Priority { get; }
    public IReadOnlyList<CommandParameter<TContext>> Parameters { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    internal CommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, CommandAttribute attribute, CommandServiceConfiguration<TContext> configuration, bool useModulePreconditions)
    {
        Aliases = attribute.Aliases.ToArray();

        Priority = attribute.Priority;

        var parameters = GetParameters(method.GetParameters(), method, configuration);
        Parameters = parameters;

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(useModulePreconditions ? [declaringType, method] : [method]);
    }

    internal CommandInfo(CommandBuilder builder, CommandServiceConfiguration<TContext> configuration)
    {
        Aliases = builder.Aliases.ToArray();

        Priority = builder.Priority;

        var handler = builder.Handler;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = GetParameters(split.Parameters, method, configuration);
        Parameters = parameters;

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    private static CommandParameter<TContext>[] GetParameters(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, CommandServiceConfiguration<TContext> configuration)
    {
        var parametersLength = parameters.Length;

        var result = new CommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];

            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException("Optional parameters must appear after all required parameters.", method);

            result[i] = new(parameter, method, configuration);
        }

        return result;
    }

    [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode", Justification = "The type of the array is known to be present")]
    public async ValueTask<CommandExecutionResult> InvokeAsync(ReadOnlyMemory<char> arguments, TContext context, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return new(preconditionResult, true);

        var separators = configuration.ParameterSeparatorsSearchValues;

        var parameters = Parameters;

        var parameterCount = parameters.Count;
        var parsedParameters = new object?[parameterCount];

        for (int i = 0; i < parameterCount; i++)
        {
            var parameter = parameters[i];

            if (arguments.IsEmpty)
            {
                if (parameter.IsOptional)
                {
                    parsedParameters[i] = parameter.DefaultValue;
                    continue;
                }
                else
                    return new(ParameterCountMismatchResult.TooFew, true);
            }

            if (parameter.Params)
            {
                List<object?> values = [];

                do
                {
                    var typeReaderResult = await parameter.ReadAsync(arguments, context, configuration, serviceProvider).ConfigureAwait(false);

                    if (typeReaderResult is not CommandTypeReaderSuccessResult typeReaderSuccessResult)
                        return new(typeReaderResult, true);

                    var value = typeReaderSuccessResult.Value;

                    var parameterPreconditionResult = await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
                    if (parameterPreconditionResult is IFailResult)
                        return new(parameterPreconditionResult, true);

                    values.Add(value);
                    arguments = arguments[typeReaderSuccessResult.Read..].TrimStart(separators);
                }
                while (!arguments.IsEmpty);

                var count = values.Count;
                var array = Array.CreateInstance(parameter.ElementType, count);

                for (int j = 0; j < count; j++)
                    array.SetValue(values[j], j);

                parsedParameters[i] = array;
                break;
            }
            else
            {
                var typeReaderResult = await parameter.ReadAsync(arguments, context, configuration, serviceProvider).ConfigureAwait(false);

                if (typeReaderResult is not CommandTypeReaderSuccessResult typeReaderSuccessResult)
                {
                    if (parameter.IsOptional)
                    {
                        parsedParameters[i] = parameter.DefaultValue;
                        continue;
                    }
                    else
                        return new(typeReaderResult, true);
                }

                var value = typeReaderSuccessResult.Value;

                var parameterPreconditionResult = await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
                if (parameterPreconditionResult is IFailResult)
                    return new(parameterPreconditionResult, true);

                parsedParameters[i] = value;
                arguments = arguments[typeReaderSuccessResult.Read..].TrimStart(separators);
            }
        }

        if (!arguments.IsEmpty)
            return new(ParameterCountMismatchResult.TooMany, true);

        try
        {
            await _invokeAsync(parsedParameters, context, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new(new ExecutionExceptionResult(ex), false);
        }

        return new(SuccessResult.Instance, false);
    }
}
