using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public class ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext
{
    public ApplicationCommandServiceConfiguration<TContext> Configuration { get; set; } = ApplicationCommandServiceConfiguration<TContext>.Default;

    public bool UseScopes { get; set; } = true;

    public Func<TInteraction, GatewayClient?, IServiceProvider, TContext>? CreateContext { get; set; }

    public IApplicationCommandResultHandler<TContext> ResultHandler { get; set; } = new ApplicationCommandResultHandler<TContext>();
}

public class ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext> : ApplicationCommandServiceOptions<TInteraction, TContext> where TInteraction : ApplicationCommandInteraction where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
{
    public Func<AutocompleteInteraction, GatewayClient?, IServiceProvider, TAutocompleteContext>? CreateAutocompleteContext { get; set; }

    public IAutocompleteInteractionResultHandler<TAutocompleteContext> AutocompleteResultHandler { get; set; } = new AutocompleteInteractionResultHandler<TAutocompleteContext>();
}
