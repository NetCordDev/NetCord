namespace NetCord.Services.ApplicationCommands;

public readonly record struct RegisteredApplicationCommand<TContext>(ulong Id, ApplicationCommandInfo<TContext> Info) where TContext : IApplicationCommandContext;
