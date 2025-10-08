using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class MessageCommandInfo<TContext> : ApplicationCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal MessageCommandInfo(MethodInfo method,
                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType,
                                MessageCommandAttribute attribute,
                                ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration, method, out var methodAttributes)
    {
        var messageParameter = _messageParameter = MethodHelper.EnsureSingleParameterOfTypeOrNone(method, typeof(RestMessage));

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes, declaringType);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, messageParameter ? [typeof(RestMessage)] : [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    internal MessageCommandInfo(MessageCommandBuilder builder,
                                ApplicationCommandServiceConfiguration<TContext> configuration) : base(builder,
                                                                                                       configuration,
                                                                                                       builder.Handler.Method,
                                                                                                       out var methodAttributes)
    {
        var handler = builder.Handler;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var messageParameter = _messageParameter = MethodHelper.EnsureSingleParameterOfTypeOrNone(split.Parameters, method, typeof(RestMessage));

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method, methodAttributes);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, messageParameter ? [typeof(RestMessage)] : [], configuration.ResultResolverProvider, configuration.ServiceResolverProvider);
    }

    private readonly bool _messageParameter;

    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override ApplicationCommandType Type => ApplicationCommandType.Message;

    public override async ValueTask<IExecutionResult> InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var preconditionResult = await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        if (preconditionResult is IFailResult)
            return preconditionResult;

        object?[]? parameters = _messageParameter ? [((MessageCommandInteraction)context.Interaction).Data.TargetMessage] : null;

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
        return new MessageCommandProperties(Name)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
            DefaultGuildPermissions = DefaultGuildPermissions,
            IntegrationTypes = IntegrationTypes,
            Contexts = Contexts,
            Nsfw = Nsfw,
        };
    }
}
