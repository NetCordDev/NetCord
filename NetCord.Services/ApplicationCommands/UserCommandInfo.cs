using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class UserCommandInfo<TContext> : ApplicationCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal UserCommandInfo(MethodInfo method,
                             [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType,
                             UserCommandAttribute attribute,
                             ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        var userParameter = _userParameter = MethodHelper.EnsureSingleParameterOfTypeOrNone(method, typeof(User));

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, userParameter ? [typeof(User)] : [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    internal UserCommandInfo(UserCommandBuilder builder,
                             ApplicationCommandServiceConfiguration<TContext> configuration) : base(builder,
                                                                                                    configuration)
    {
        var handler = builder.Handler;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var userParameter = _userParameter = MethodHelper.EnsureSingleParameterOfTypeOrNone(split.Parameters, method, typeof(User));

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, userParameter ? [typeof(User)] : [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    private readonly bool _userParameter;

    public override ApplicationCommandType Type => ApplicationCommandType.User;
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override async ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        object?[]? parameters = _userParameter ? [((UserCommandInteraction)context.Interaction).Data.TargetUser] : null;

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
        return new UserCommandProperties(Name)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildPermissions = DefaultGuildPermissions,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
        };
    }
}
