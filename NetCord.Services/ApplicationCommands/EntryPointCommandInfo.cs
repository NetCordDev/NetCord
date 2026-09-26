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
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration, method, out var methodAttributes)
    {
        Description = attribute.Description;

        Handler = EntryPointCommandHandler.ApplicationHandler;

        MethodHelper.EnsureNoParameters(method);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes, declaringType);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    internal EntryPointCommandInfo(EntryPointCommandBuilder builder,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(builder,
                                                                                                          configuration,
                                                                                                          builder.Handler?.Method,
                                                                                                          out var methodAttributes)
    {
        Description = builder.Description;

        var handler = builder.Handler;

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

            Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes);

            _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
        }
    }

    internal EntryPointCommandInfo(Type type,
                                   EntryPointCommandAttribute attribute,
                                   ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute,
                                                                                                          configuration,
                                                                                                          type,
                                                                                                          out _)
    {
        Description = attribute.Description;

        Handler = EntryPointCommandHandler.DiscordLaunchActivity;

        Preconditions = [];

        _invokeAsync = EmptyInvokeAsync;
    }

    private static ValueTask EmptyInvokeAsync(object?[]? parameters, TContext context, IServiceProvider? serviceProvider) => default;

    public override ApplicationCommandType Type => ApplicationCommandType.EntryPoint;
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
        return new EntryPointCommandProperties(Name, Description, Handler)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DescriptionLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(DescriptionLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildPermissions = DefaultGuildPermissions,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
        };
    }
}
