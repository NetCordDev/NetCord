using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class SlashCommandInfo<TContext> : ApplicationCommandInfo<TContext>, IAutocompleteInfo where TContext : IApplicationCommandContext
{
    internal SlashCommandInfo(MethodInfo method,
                              [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType,
                              SlashCommandAttribute attribute,
                              ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration, method, out var methodAttributes)
    {
        Description = attribute.Description;

        var parameters = SlashCommandParametersHelper.GetParameters(method.GetParameters(), method, configuration, LocalizationPath);
        Parameters = parameters;
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes, declaringType);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    internal SlashCommandInfo(SlashCommandBuilder builder,
                              ApplicationCommandServiceConfiguration<TContext> configuration) : base(builder,
                                                                                                     configuration,
                                                                                                     builder.Handler.Method,
                                                                                                     out var methodAttributes)
    {
        Description = builder.Description;

        var handler = builder.Handler;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = SlashCommandParametersHelper.GetParameters(split.Parameters, method, configuration, LocalizationPath);
        Parameters = parameters;
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    public override ApplicationCommandType Type => ApplicationCommandType.ChatInput;
    public string Description { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>> Parameters { get; }
    public IReadOnlyDictionary<string, SlashCommandParameter<TContext>> ParametersDictionary { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override async ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        var parameters = new object?[Parameters.Count];
        var result = await SlashCommandParametersHelper.ParseParametersAsync(context, ((SlashCommandInteraction)context.Interaction).Data.Options, Parameters, configuration, serviceProvider, parameters).ConfigureAwait(false);
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

    public override async ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default)
    {
        var parameters = Parameters;
        var count = parameters.Count;

        var options = new ApplicationCommandOptionProperties[count];
        for (int i = 0; i < count; i++)
            options[i] = await parameters[i].GetRawValueAsync(cancellationToken).ConfigureAwait(false);

        return new SlashCommandProperties(Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildPermissions = DefaultGuildPermissions,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
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

        return NotFoundResult.Command;
    }

    void IAutocompleteInfo.InitializeAutocomplete<TAutocompleteContext>(IServiceResolverProvider serviceResolverProvider)
    {
        var parameters = Parameters;
        var count = parameters.Count;
        for (int i = 0; i < count; i++)
            parameters[i].InitializeAutocomplete<TAutocompleteContext>(serviceResolverProvider);
    }
}
