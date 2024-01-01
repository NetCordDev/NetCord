using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SubSlashCommandInfo<TContext> : ISubSlashCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal SubSlashCommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, SubSlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        Name = attribute.Name!;

        var nameTranslationsProviderType = attribute.NameTranslationsProviderType;
        if (nameTranslationsProviderType is not null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;

        Description = attribute.Description!;

        var descriptionTranslationsProviderType = attribute.DescriptionTranslationsProviderType;
        if (descriptionTranslationsProviderType is not null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(descriptionTranslationsProviderType)!;

        var parameters = Parameters = SlashCommandParametersHelper.GetParameters(method.GetParameters(), method, configuration);
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>> Parameters { get; }
    public IReadOnlyDictionary<string, SlashCommandParameter<TContext>> ParametersDictionary { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public async ValueTask<IExecutionResult> InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        var parameters = new object?[Parameters.Count];
        var result = await SlashCommandParametersHelper.ParseParametersAsync(context, options, Parameters, configuration, serviceProvider, parameters).ConfigureAwait(false);
        if (result is IFailResult)
            return result;

        try
        {
            await _invokeAsync(parameters, context, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExecutionExceptionResult(ex);
        }

        return SuccessResult.Instance;
    }

    public ApplicationCommandOptionProperties GetRawValue()
    {
        return new(ApplicationCommandOptionType.SubCommand, Name, Description)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DescriptionLocalizations = DescriptionTranslationsProvider?.Translations,
            Options = Parameters.Select(p => p.GetRawValue())
        };
    }

    public async ValueTask<IExecutionResult> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var option = options.First(o => o.Focused);
        if (ParametersDictionary.TryGetValue(option.Name, out var parameter))
        {
            try
            {
                var result = await parameter.InvokeAutocompleteAsync(context, option, serviceProvider).ConfigureAwait(false);
                await context.Interaction.SendResponseAsync(InteractionCallback.Autocomplete(result)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return new ExecutionExceptionResult(ex);
            }

            return SuccessResult.Instance;
        }

        return new NotFoundResult("Command not found.");
    }

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>()
    {
        var parameters = Parameters;
        var count = parameters.Count;
        for (int i = 0; i < count; i++)
            parameters[i].InitializeAutocomplete<TAutocompleteContext>();
    }
}
