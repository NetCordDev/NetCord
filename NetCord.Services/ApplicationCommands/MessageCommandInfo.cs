﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class MessageCommandInfo<TContext> : ApplicationCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal MessageCommandInfo(MethodInfo method,
                                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType,
                                MessageCommandAttribute attribute,
                                ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        MethodHelper.EnsureNoParameters(method);

        _invokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, Enumerable.Empty<Type>(), configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    internal MessageCommandInfo(string name,
                             Delegate handler,
                             [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? nameTranslationsProviderType,
                             Permissions? defaultGuildUserPermissions,
                             bool? dMPermission,
                             bool defaultPermission,
                             bool nsfw,
                             ulong? guildId,
                             ApplicationCommandServiceConfiguration<TContext> configuration) : base(name,
                                                                                                    nameTranslationsProviderType,
                                                                                                    defaultGuildUserPermissions,
                                                                                                    dMPermission,
                                                                                                    defaultPermission,
                                                                                                    nsfw,
                                                                                                    guildId,
                                                                                                    configuration)
    {
        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);
        MethodHelper.EnsureNoParameters(split.Parameters, method);

        _invokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, Enumerable.Empty<Type>(), configuration.ResultResolverProvider);
        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

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

    public override ApplicationCommandProperties GetRawValue()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return new MessageCommandProperties(Name)
        {
            NameLocalizations = NameTranslationsProvider?.Translations,
            DefaultGuildUserPermissions = DefaultGuildUserPermissions,
            DMPermission = DMPermission,
            DefaultPermission = DefaultPermission,
            Nsfw = Nsfw,
        };
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
