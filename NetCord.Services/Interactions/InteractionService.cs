using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionService<TContext> where TContext : IInteractionContext
{
    private readonly InteractionServiceConfiguration<TContext> _configuration;
    private readonly Dictionary<ReadOnlyMemory<char>, InteractionInfo<TContext>> _interactions = new(ReadOnlyMemoryCharComparer.InvariantCulture);

    public IReadOnlyDictionary<ReadOnlyMemory<char>, InteractionInfo<TContext>> GetInteractions() => new Dictionary<ReadOnlyMemory<char>, InteractionInfo<TContext>>(_interactions);

    public InteractionService(InteractionServiceConfiguration<TContext>? configuration = null)
    {
        _configuration = configuration ?? new();
    }

    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly)
    {
        var baseType = typeof(BaseInteractionModule<TContext>);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAssignableTo(baseType))
                AddModuleCore(type);
        }
    }

    public void AddModule([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        if (!type.IsAssignableTo(typeof(BaseInteractionModule<TContext>)))
            throw new InvalidOperationException($"Modules must inherit from '{nameof(BaseInteractionModule<TContext>)}'.");

        AddModuleCore(type);
    }

    public void AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] T>()
    {
        AddModule(typeof(T));
    }

    private void AddModuleCore([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
    {
        var configuration = _configuration;
        foreach (var method in type.GetMethods())
        {
            InteractionAttribute? interactionAttribute = method.GetCustomAttribute<InteractionAttribute>();
            if (interactionAttribute is null)
                continue;
            InteractionInfo<TContext> interactionInfo = new(method, type, configuration);
            _interactions.Add(interactionAttribute.CustomId.AsMemory(), interactionInfo);
        }
    }

    public async Task ExecuteAsync(TContext context, IServiceProvider? serviceProvider = null)
    {
        var configuration = _configuration;
        var separator = configuration.ParameterSeparator;
        var content = ((ICustomIdInteractionData)context.Interaction.Data).CustomId.AsMemory();
        var index = content.Span.IndexOf(separator);
        InteractionInfo<TContext> interactionInfo;
        ReadOnlyMemory<char> arguments;
        if (index == -1)
        {
            var customId = content;
            interactionInfo = GetInteractionInfo(customId);
            arguments = default;
        }
        else
        {
            var customId = content[..index];
            interactionInfo = GetInteractionInfo(customId);
            arguments = content[(index + 1)..];
        }

        await interactionInfo.EnsureCanExecuteAsync(context, serviceProvider).ConfigureAwait(false);

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
                    value = await parameter.TypeReader.ReadAsync(currentArg, context, parameter, configuration, serviceProvider).ConfigureAwait(false);
                    await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
                }

                parametersToPass[paramIndex] = value;
            }
            else
            {
                if (parameter.HasDefaultValue && arguments.IsEmpty)
                    parametersToPass[paramIndex] = parameter.DefaultValue;
                else
                    await ReadParamsAsync(context, separator, parametersToPass, arguments, paramIndex, parameter, configuration, serviceProvider).ConfigureAwait(false);
            }
        }

        await interactionInfo.InvokeAsync(parametersToPass, context, serviceProvider).ConfigureAwait(false);
    }

    private static async Task ReadParamsAsync(TContext context, char separator, object?[] parametersToPass, ReadOnlyMemory<char> arguments, int paramIndex, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var ranges = Split(arguments.Span, separator);
        var count = ranges.Count;
        var array = Array.CreateInstance(parameter.ElementType, count);
        for (int i = 0; i < count; i++)
        {
            var value = await parameter.TypeReader.ReadAsync(arguments[ranges[i]], context, parameter, configuration, serviceProvider).ConfigureAwait(false);
            await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
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

    private InteractionInfo<TContext> GetInteractionInfo(ReadOnlyMemory<char> customId)
    {
        if (_interactions.TryGetValue(customId, out var interactionInfo))
            return interactionInfo;

        throw new InteractionNotFoundException();
    }
}
