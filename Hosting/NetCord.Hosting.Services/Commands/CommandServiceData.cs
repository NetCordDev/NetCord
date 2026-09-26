using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

internal record CommandServiceData(ICommandService Service, ICommandsBuilder Builder);
