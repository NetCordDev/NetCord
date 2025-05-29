using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
#if (addApplicationCommands)
using NetCord.Hosting.Services.ApplicationCommands;
#endif
#if (addTextCommands)
using NetCord.Hosting.Services.Commands;
#endif
#if (addComponentInteractions)
using NetCord.Hosting.Services.ComponentInteractions;
#endif
#if (addComponentInteractions)
using NetCord.Services.ComponentInteractions;
#endif

namespace NetCord.Template.Bot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services
            // Use intents suitable for your needs.
            // See: https://netcord.dev/guides/events/intents.html?tabs=generic-host
            .AddDiscordGateway(op => op.Intents = GatewayIntents.All)
#if (addTextCommands)
            .AddApplicationCommands()
#endif
#if (addTextCommands)
            .AddCommands(options => options.IgnoreCase = true) // IgnoreCase makes commands case-insensitive. "!hello" and "!Hello" will be treated the same.
#endif
#if (addComponentInteractions)
            .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
            .AddComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
            .AddComponentInteractions<UserMenuInteraction, UserMenuInteractionContext>()
            .AddComponentInteractions<RoleMenuInteraction, RoleMenuInteractionContext>()
            .AddComponentInteractions<MentionableMenuInteraction, MentionableMenuInteractionContext>()
            .AddComponentInteractions<ChannelMenuInteraction, ChannelMenuInteractionContext>()
            .AddComponentInteractions<ModalInteraction, ModalInteractionContext>()
#endif
            .AddGatewayEventHandlers(typeof(Program).Assembly);

        var host = builder
            .Build()
            // Adds application commands, text commands and interactions from the assembly.
            // All "Modules" must be public in order to be discovered and registered properly.
            .AddModules(typeof(Program).Assembly)
            .UseGatewayEventHandlers();

        await host.RunAsync().ConfigureAwait(false);
    }
}
