using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.ApplicationCommands;

internal readonly struct AutocompleteDelegateProvider<TContext>(ApplicationCommandService<TContext> service) where TContext : IApplicationCommandContext
{
    public Delegate Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type autocompleteProviderType,
                           IServiceResolverProvider serviceResolverProvider,
                           MethodInfo method)
    {
        return service.CreateAutocompleteDelegate(autocompleteProviderType, serviceResolverProvider, method);
    }
}
