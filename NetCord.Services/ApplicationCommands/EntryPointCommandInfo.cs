using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

internal class EntryPointCommandInfo<TContext> : ApplicationCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal EntryPointCommandInfo(MethodInfo method,
                                   [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType,
                                   EntryPointCommandAttribute attribute,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        Description = attribute.Description;

        Handler = EntryPointCommandHandler.ApplicationHandler;

        MethodHelper.EnsureNoParameters(method);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    internal EntryPointCommandInfo(string name,
                                   string description,
                                   Delegate? handler,
                                   Permissions? defaultGuildUserPermissions,
                                   bool? dMPermission,
                                   bool defaultPermission,
                                   IEnumerable<ApplicationIntegrationType>? integrationTypes,
                                   IEnumerable<InteractionContextType>? contexts,
                                   bool nsfw,
                                   bool register,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(name,
                                                                                                          defaultGuildUserPermissions,
                                                                                                          dMPermission,
                                                                                                          defaultPermission,
                                                                                                          integrationTypes,
                                                                                                          contexts,
                                                                                                          nsfw,
                                                                                                          register,
                                                                                                          configuration)
    {
        Description = description;

        if (handler is null)
        {
            Handler = EntryPointCommandHandler.DiscordLaunchActivity;
            Preconditions = [];
            _invokeAsync = EmptyInvokeAsync;
        }
        else
        {
            Handler = EntryPointCommandHandler.ApplicationHandler;

            var method = handler.Method;

            var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

            MethodHelper.EnsureNoParameters(split.Parameters, method);

            Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);

            _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
        }
    }

    internal EntryPointCommandInfo(EntryPointCommandAttribute attribute,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute.Name,
                                                                                                          attribute._defaultGuildUserPermissions,
                                                                                                          attribute._dMPermission,
#pragma warning disable CS0618 // Type or member is obsolete
                                                                                                          attribute.DefaultPermission,
#pragma warning restore CS0618 // Type or member is obsolete
                                                                                                          attribute.IntegrationTypes,
                                                                                                          attribute.Contexts,
                                                                                                          attribute.Nsfw,
                                                                                                          attribute.Register,
                                                                                                          configuration)
    {
        Description = attribute.Description;

        Handler = EntryPointCommandHandler.DiscordLaunchActivity;

        Preconditions = [];

        _invokeAsync = EmptyInvokeAsync;
    }

    private static ValueTask EmptyInvokeAsync(object?[]? parameters, TContext context, IServiceProvider? serviceProvider) => default;

    public string Description { get; }
    public EntryPointCommandHandler Handler { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override async ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        try
        {
            await _invokeAsync(null, context, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ExecutionExceptionResult(ex);
        }

        return SuccessResult.Instance;
    }

    public override async ValueTask<ApplicationCommandProperties> GetRawValueAsync(CancellationToken cancellationToken = default)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return new EntryPointCommandProperties(Name, Description, Handler)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildUserPermissions = DefaultGuildUserPermissions,
            DMPermission = DMPermission,
            DefaultPermission = DefaultPermission,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
        };
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
