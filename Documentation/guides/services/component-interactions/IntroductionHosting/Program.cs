using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .AddComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
    .AddComponentInteractions<UserMenuInteraction, UserMenuInteractionContext>()
    .AddComponentInteractions<RoleMenuInteraction, RoleMenuInteractionContext>()
    .AddComponentInteractions<MentionableMenuInteraction, MentionableMenuInteractionContext>()
    .AddComponentInteractions<ChannelMenuInteraction, ChannelMenuInteractionContext>()
    .AddComponentInteractions<ModalInteraction, ModalInteractionContext>();

var host = builder.Build();

// Add component interactions using minimal APIs
host.AddComponentInteraction<ButtonInteractionContext>("ping", () => "Pong!")
    .AddComponentInteraction<StringMenuInteractionContext>("string", (StringMenuInteractionContext context) => string.Join("\n", context.SelectedValues))
    .AddComponentInteraction<UserMenuInteractionContext>("user", (UserMenuInteractionContext context) => string.Join("\n", context.SelectedUsers))
    .AddComponentInteraction<RoleMenuInteractionContext>("role", (RoleMenuInteractionContext context) => string.Join("\n", context.SelectedRoles))
    .AddComponentInteraction<MentionableMenuInteractionContext>("mentionable", (MentionableMenuInteractionContext context) => string.Join("\n", context.SelectedMentionables))
    .AddComponentInteraction<ChannelMenuInteractionContext>("channel", (ChannelMenuInteractionContext context) => string.Join("\n", context.SelectedChannels))
    .AddComponentInteraction<ModalInteractionContext>("modal", (ModalInteractionContext context) => ((TextInput)context.Components[0]).Value);

// Add component interactions from modules
host.AddModules(typeof(Program).Assembly);

// Add handlers to handle the component interactions
host.UseGatewayEventHandlers();

await host.RunAsync();
