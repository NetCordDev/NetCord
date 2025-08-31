
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

internal record ApplicationCommandServiceData(IApplicationCommandService Service, IApplicationCommandsBuilder Builder, Func<bool?> AutoRegisterCommandsFunc);
