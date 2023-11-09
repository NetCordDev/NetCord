using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Rest;
using NetCord.Services.Helpers;

namespace NetCord.Services.ApplicationCommands;

public class UserCommandInfo<TContext> : ApplicationCommandInfo<TContext> where TContext : IApplicationCommandContext
{
    internal UserCommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, UserCommandAttribute attribute, ApplicationCommandServiceConfiguration<TContext> configuration) : base(attribute, configuration)
    {
        MethodHelper.EnsureMethodParameterless(method);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
        _invokeAsync = InvocationHelper.CreateDelegate(method, declaringType, Enumerable.Empty<Type>(), configuration.ResultResolverProvider);
    }

    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    private readonly Func<object?[]?, TContext, IServiceProvider?, ValueTask> _invokeAsync;

    public override async ValueTask InvokeAsync(TContext context, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        await PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider).ConfigureAwait(false);
        await _invokeAsync(null, context, serviceProvider).ConfigureAwait(false);
    }

    public override ApplicationCommandProperties GetRawValue()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return new UserCommandProperties(Name)
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
