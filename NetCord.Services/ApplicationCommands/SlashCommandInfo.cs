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
                              ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        Description = attribute.Description;

        var parameters = SlashCommandParametersHelper.GetParameters(method.GetParameters(), method, configuration, LocalizationPath);
        Parameters = parameters;
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider);
    }

    internal SlashCommandInfo(string name,
                              string description,
                              Delegate handler,
                              Permissions? defaultGuildUserPermissions,
                              bool? dMPermission,
                              bool defaultPermission,
                              bool nsfw,
                              ulong? guildId,
                              ApplicationCommandServiceConfiguration<TContext> configuration) : base(name,
                                                                                                     defaultGuildUserPermissions,
                                                                                                     dMPermission,
                                                                                                     defaultPermission,
                                                                                                     nsfw,
                                                                                                     guildId,
                                                                                                     configuration)
    {
        Description = description;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = SlashCommandParametersHelper.GetParameters(split.Parameters, method, configuration, LocalizationPath);
        Parameters = parameters;
        ParametersDictionary = parameters.ToFrozenDictionary(p => p.Name);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    public string Description { get; }
    public IReadOnlyList<SlashCommandParameter<TContext>> Parameters { get; }
    public IReadOnlyDictionary<string, SlashCommandParameter<TContext>> ParametersDictionary { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override LocalizationPathSegment LocalizationPathSegment => new ApplicationCommandLocalizationPathSegment(Name);

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

    public override async ValueTask<ApplicationCommandProperties> GetRawValueAsync()
    {
        var parameters = Parameters;
        var count = parameters.Count;

        var options = new ApplicationCommandOptionProperties[count];
        for (int i = 0; i < count; i++)
            options[i] = await parameters[i].GetRawValueAsync().ConfigureAwait(false);

#pragma warning disable CS0618 // Type or member is obsolete
        return new SlashCommandProperties(Name, Description)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance)).ConfigureAwait(false),
            DefaultGuildUserPermissions = DefaultGuildUserPermissions,
            DMPermission = DMPermission,
            DefaultPermission = DefaultPermission,
            Nsfw = Nsfw,
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance)).ConfigureAwait(false),
            Options = options,
        };
#pragma warning restore CS0618 // Type or member is obsolete
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
