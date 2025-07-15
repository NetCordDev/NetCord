using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

internal readonly record struct RegisteredApplicationCommandInfo(ApplicationCommand Command, IApplicationCommandInfo CommandInfo);

public readonly record struct RegisteredApplicationCommandInfo<TContext>(ApplicationCommand Command, ApplicationCommandInfo<TContext> CommandInfo) where TContext : IApplicationCommandContext;
