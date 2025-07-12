namespace NetCord.Services.ApplicationCommands;

public readonly record struct RegisteredApplicationCommandInfo<TContext>(ulong CommandId, ApplicationCommandInfo<TContext> CommandInfo) where TContext : IApplicationCommandContext;
