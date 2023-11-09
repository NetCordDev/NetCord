using System.Reflection;

namespace NetCord.Services.ApplicationCommands;

internal static class SlashCommandParametersHelper
{
    public static SlashCommandParameter<TContext>[] GetParameters<TContext>(MethodInfo method, ApplicationCommandServiceConfiguration<TContext> configuration) where TContext : IApplicationCommandContext
    {
        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;
        var p = new SlashCommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", method);

            SlashCommandParameter<TContext> newP = new(parameter, method, configuration);
            p[i] = newP;
        }

        return p;
    }

    public static async ValueTask<object?[]> ParseParametersAsync<TContext>(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IReadOnlyList<SlashCommandParameter<TContext>> parameters, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) where TContext : IApplicationCommandContext
    {
        int parametersCount = parameters.Count;
        var parametersToPass = new object?[parametersCount];
        int optionsCount = options.Count;
        int parameterIndex = 0;
        for (int optionIndex = 0; optionIndex < optionsCount; parameterIndex++)
        {
            var parameter = parameters[parameterIndex];
            var option = options[optionIndex];
            object? value;
            if (parameter.Name == option.Name)
            {
                value = await parameter.TypeReader.ReadAsync(option.Value!, context, parameter, configuration, serviceProvider).ConfigureAwait(false);
                await parameter.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
                optionIndex++;
            }
            else
                value = parameter.DefaultValue;

            parametersToPass[parameterIndex] = value;
        }
        while (parameterIndex < parametersCount)
        {
            var parameter = parameters[parameterIndex];
            parametersToPass[parameterIndex] = parameter.DefaultValue;
            parameterIndex++;
        }

        return parametersToPass;
    }
}
