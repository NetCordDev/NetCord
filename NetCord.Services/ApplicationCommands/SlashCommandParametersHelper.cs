using System.Collections.Immutable;
using System.Reflection;

namespace NetCord.Services.ApplicationCommands;

internal static class SlashCommandParametersHelper
{
    public static SlashCommandParameter<TContext>[] GetParameters<TContext>(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration, ImmutableList<LocalizationPathSegment> path) where TContext : IApplicationCommandContext
    {
        var parametersLength = parameters.Length;
        var result = new SlashCommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException("Optional parameters must appear after all required parameters.", method);

            result[i] = new(parameter, method, configuration, path);
        }

        return result;
    }

    public static async ValueTask<IExecutionResult> ParseParametersAsync<TContext>(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IReadOnlyList<SlashCommandParameter<TContext>> parameters, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider, object?[] parametersToPass) where TContext : IApplicationCommandContext
    {
        int optionsCount = options.Count;
        int parametersCount = parameters.Count;
        int parameterIndex = 0;

        for (int optionIndex = 0; optionIndex < optionsCount; parameterIndex++)
        {
            if (parameterIndex >= parametersCount)
                return ParametersMismatchResult.SlashCommandNotMatching;

            var parameter = parameters[parameterIndex];
            var option = options[optionIndex];

            object? value;
            if (parameter.Name == option.Name)
            {
                SlashCommandTypeReaderResult typeReaderResult;
                try
                {
                    typeReaderResult = await parameter.TypeReader.ReadAsync(option.Value!, context, parameter, configuration, serviceProvider).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    return new SlashCommandTypeReaderExceptionResult(ex);
                }

                if (typeReaderResult is not SlashCommandTypeReaderSuccessResult typeReaderSuccessResult)
                    return typeReaderResult;

                value = typeReaderSuccessResult.Value;

                var preconditionResult = await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);

                if (preconditionResult is IFailResult)
                    return preconditionResult;

                optionIndex++;
            }
            else if (parameter.IsOptional)
                value = parameter.DefaultValue;
            else
                return ParametersMismatchResult.SlashCommandNotMatching;

            parametersToPass[parameterIndex] = value;
        }

        while (parameterIndex < parametersCount)
        {
            var parameter = parameters[parameterIndex];

            if (!parameter.IsOptional)
                return ParametersMismatchResult.SlashCommandNotMatching;

            parametersToPass[parameterIndex] = parameter.DefaultValue;
            parameterIndex++;
        }

        return SuccessResult.Instance;
    }
}
