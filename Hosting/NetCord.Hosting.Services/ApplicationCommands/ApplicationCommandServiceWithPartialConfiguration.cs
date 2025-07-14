
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal record ApplicationCommandServiceWithPartialConfiguration(IApplicationCommandService Service, Func<bool?> AutoRegisterCommandsFunc);
