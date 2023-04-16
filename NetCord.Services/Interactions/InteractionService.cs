using System.Reflection;

using NetCord.Gateway;

namespace NetCord.Services.Interactions;

public class InteractionService<TContext> : IService where TContext : InteractionContext
{
    private readonly InteractionServiceConfiguration<TContext> _configuration;
    private readonly Dictionary<string, InteractionInfo<TContext>> _interactions = new();

    public IReadOnlyDictionary<string, InteractionInfo<TContext>> GetInteractions()
    {
        lock (_interactions)
            return new Dictionary<string, InteractionInfo<TContext>>(_interactions);
    }

    public InteractionService(InteractionServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ?? new();
    }

    public void AddModules(Assembly assembly)
    {
        Type baseType = typeof(BaseInteractionModule<TContext>);
        lock (_interactions)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(baseType))
                    AddModuleCore(type);
            }
        }
    }

    public void AddModule(Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseInteractionModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseInteractionModule<TContext>)}'.");

        lock (_interactions)
            AddModuleCore(type);
    }

    private void AddModuleCore(Type type)
    {
        var configuration = _configuration;
        foreach (var method in type.GetMethods())
        {
            InteractionAttribute? interactionAttribute = method.GetCustomAttribute<InteractionAttribute>();
            if (interactionAttribute == null)
                continue;
            InteractionInfo<TContext> interactionInfo = new(method, configuration);
            _interactions.Add(interactionAttribute.CustomId, interactionInfo);
        }
    }

    public async Task ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        var configuration = _configuration;
        var separator = configuration.ParameterSeparator;
        var content = ((ICustomIdInteractionData)context.Interaction.Data).CustomId;
        var index = content.IndexOf(separator);
        string? customId;
        ReadOnlyMemory<char> arguments;
        if (index == -1)
        {
            customId = content;
            arguments = default;
        }
        else
        {
            customId = content[..index];
            arguments = content.AsMemory(index + 1);
        }
        var interactionInfo = GetInteractionInfo(customId);

        await interactionInfo.EnsureCanExecuteAsync(context).ConfigureAwait(false);

        var interactionParameters = interactionInfo.Parameters;
        int interactionParametersLength = interactionParameters.Count;

        var parametersToPass = new object?[interactionParametersLength];

        var maxParamIndex = interactionParametersLength - 1;
        for (int paramIndex = 0; paramIndex <= maxParamIndex; paramIndex++)
        {
            var parameter = interactionParameters[paramIndex];
            if (!parameter.Params)
            {
                ReadOnlyMemory<char> currentArg;
                if (paramIndex == maxParamIndex)
                    currentArg = arguments;
                else
                {
                    index = arguments.Span.IndexOf(separator);
                    if (index == -1)
                        throw new ParameterCountException(ParameterCountExceptionType.TooFew);

                    currentArg = arguments[..index];
                    arguments = arguments[(index + 1)..];
                }
                object? value;
                if (parameter.HasDefaultValue && currentArg.IsEmpty)
                    value = parameter.DefaultValue;
                else
                {
                    value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, configuration).ConfigureAwait(false);
                    await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
                }

                parametersToPass[paramIndex] = value;
            }
            else
            {
                if (parameter.HasDefaultValue && arguments.IsEmpty)
                    parametersToPass[paramIndex] = parameter.DefaultValue;
                else
                    await ReadParamsAsync(context, separator, parametersToPass, arguments, paramIndex, parameter, configuration).ConfigureAwait(false);
            }
        }

        await interactionInfo.InvokeAsync(parametersToPass, context, serviceProvider).ConfigureAwait(false);
    }

    private static async Task ReadParamsAsync(TContext context, char separator, object?[] parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration)
    {
        var ranges = Split(arguments.Span, separator);
        var count = ranges.Count;
        var array = Array.CreateInstance(parameter.ElementType, count);
        for (int i = 0; i < count; i++)
        {
            var value = await parameter.TypeReader.ReadAsync(arguments[ranges[i]], context, parameter, configuration).ConfigureAwait(false);
            await parameter.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
            array.SetValue(value, i);
        }
        parametersToPass[paramIndex] = array;

        static List<Range> Split(ReadOnlySpan<char> arguments, char separator)
        {
            List<Range> result = new();

            int startIndex = 0;
            int index;

            while ((index = arguments.IndexOf(separator)) != -1)
            {
                result.Add(new(startIndex, startIndex + index));
                var move = index + 1;
                startIndex += move;
                arguments = arguments[move..];
            }
            result.Add(new(startIndex, startIndex + arguments.Length));

            return result;
        }
    }

    private InteractionInfo<TContext> GetInteractionInfo(string customId)
    {
        InteractionInfo<TContext>? interactionInfo;
        bool success;
        lock (_interactions)
            success = _interactions.TryGetValue(customId, out interactionInfo);

        if (success)
            return interactionInfo!;
        throw new InteractionNotFoundException();
    }
}
