﻿using System.Diagnostics.CodeAnalysis;
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
        MethodHelper.EnsureNoParameters(method);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, [], configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    internal UserCommandInfo(string name,
                             Delegate handler,
                             Permissions? defaultGuildUserPermissions,
                             bool? dMPermission,
                             bool defaultPermission,
                             IEnumerable<ApplicationIntegrationType>? integrationTypes,
                             IEnumerable<InteractionContextType>? contexts,
                             bool nsfw,
                             ulong? guildId,
                             ApplicationCommandServiceConfiguration<TContext> configuration) : base(name,
                                                                                                    defaultGuildUserPermissions,
                                                                                                    dMPermission,
                                                                                                    defaultPermission,
                                                                                                    integrationTypes,
                                                                                                    contexts,
                                                                                                    nsfw,
                                                                                                    guildId,
                                                                                                    configuration)
    {
        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);
        MethodHelper.EnsureNoParameters(split.Parameters, method);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, [], configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override LocalizationPathSegment LocalizationPathSegment => new ApplicationCommandLocalizationPathSegment(Name);

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
        return new UserCommandProperties(Name)
        {
            NameLocalizations = LocalizationsProvider is null ? null : await LocalizationsProvider.GetLocalizationsAsync(LocalizationPath.Add(NameLocalizationPathSegment.Instance), cancellationToken).ConfigureAwait(false),
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
