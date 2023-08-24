using System.Reflection;
using System.Runtime.CompilerServices;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SubSlashCommandInfo<TContext> : ISubSlashCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal SubSlashCommandInfo(MethodInfo method, SubSlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        MethodHelper.EnsureMethodReturnTypeValid(method);

        Name = attribute.Name!;

        var nameTranslationsProviderType = attribute.NameTranslationsProviderType;
        if (nameTranslationsProviderType is not null)
            NameTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(nameTranslationsProviderType)!;

        Description = attribute.Description!;

        var descriptionTranslationsProviderType = attribute.DescriptionTranslationsProviderType;
        if (descriptionTranslationsProviderType is not null)
            DescriptionTranslationsProvider = (ITranslationsProvider)Activator.CreateInstance(descriptionTranslationsProviderType)!;

        var parameters = Parameters = SlashCommandParametersHelper.GetParameters(method, configuration);
        ParametersDictionary = parameters.ToDictionary(p => p.Name);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
        _invokeAsync = InvocationHelper.CreateDelegate<TContext>(method, method.DeclaringType!, parameters.Select(p => p.Type));
    }

    public string Name { get; }
    public ITranslationsProvider? NameTranslationsProvider { get; }
    public string Description { get; }
    public ITranslationsProvider? DescriptionTranslationsProvider { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>> Parameters { get; }
    public IReadOnlyDictionary<string, SlashCommandParameter<TContext>> ParametersDictionary { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, Task> _invokeAsync;

    public async Task InvokeAsync(TContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);

        var parameters = await SlashCommandParametersHelper.ParseParametersAsync(context, options, Parameters, configuration, serviceProvider).ConfigureAwait(false);
        await _invokeAsync(parameters, context, serviceProvider).ConfigureAwait(false);
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

    public Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?> InvokeAutocompleteAsync<TAutocompleteContext>(TAutocompleteContext context, IReadOnlyList<ApplicationCommandInteractionDataOption> options, IServiceProvider? serviceProvider) where TAutocompleteContext : IAutocompleteInteractionContext
    {
        var option = options[0];
        if (ParametersDictionary.TryGetValue(option.Name, out var parameter))
            return Unsafe.As<Func<ApplicationCommandInteractionDataOption, TAutocompleteContext, IServiceProvider?, Task<IEnumerable<ApplicationCommandOptionChoiceProperties>?>>>(parameter.InvokeAutocomplete!)(option, context, serviceProvider);

        throw new AutocompleteNotFoundException();
    }
}
