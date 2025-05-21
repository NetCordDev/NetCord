using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SubSlashCommandInfo<TContext> : ISubSlashCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal SubSlashCommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, SubSlashCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration, ImmutableList<LocalizationPathSegment> path)
    {
        Name = attribute.Name;

        var localizationPath = LocalizationPath = path.Add(new SubSlashCommandLocalizationPathSegment(Name));

        LocalizationsProvider = configuration.LocalizationsProvider;

        Description = attribute.Description;

        var parameters = Parameters = SlashCommandParametersHelper.GetParameters(method.GetParameters(), method, configuration, localizationPath);
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    internal SubSlashCommandInfo(string name, string description, Delegate handler, ApplicationCommandServiceConfiguration<TContext> configuration, ImmutableList<LocalizationPathSegment> path)
    {
        Name = name;

        var localizationPath = LocalizationPath = path.Add(new SubSlashCommandLocalizationPathSegment(name));

        LocalizationsProvider = configuration.LocalizationsProvider;

        Description = description;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = SlashCommandParametersHelper.GetParameters(split.Parameters, method, configuration, LocalizationPath);
        Parameters = parameters;
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    public string Name { get; }
    public ILocalizationsProvider? LocalizationsProvider { get; }
    public ImmutableList<LocalizationPathSegment> LocalizationPath { get; }
    public string Description { get; }
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

    public async ValueTask<ApplicationCommandOptionProperties> GetRawValueAsync(CancellationToken cancellationToken = default)
    {
        var parameters = Parameters;
        var count = parameters.Count;

        var options = new ApplicationCommandOptionProperties[count];
        for (int i = 0; i < count; i++)
            options[i] = await parameters[i].GetRawValueAsync(cancellationToken).ConfigureAwait(false);

        return new(ApplicationCommandOptionType.SubCommand, Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            Options = options,
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

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>(IServiceResolverProvider serviceResolverProvider)
    {
        var parameters = Parameters;
        var count = parameters.Count;
        for (int i = 0; i < count; i++)
            parameters[i].InitializeAutocomplete<TAutocompleteContext>(serviceResolverProvider);
    }
}
