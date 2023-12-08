namespace NetCord.Services.ApplicationCommands;

internal record struct GuildCommands(ulong GuildId, IReadOnlyList<IApplicationCommandInfo> Commands);
