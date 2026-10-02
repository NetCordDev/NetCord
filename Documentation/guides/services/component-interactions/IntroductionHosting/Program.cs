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
    .AddComponentInteractions<StringSelectInteraction, StringMenuInteractionContext>()
    .AddComponentInteractions<UserSelectInteraction, UserMenuInteractionContext>()
    .AddComponentInteractions<RoleSelectInteraction, RoleMenuInteractionContext>()
    .AddComponentInteractions<MentionableSelectInteraction, MentionableMenuInteractionContext>()
    .AddComponentInteractions<ChannelSelectInteraction, ChannelMenuInteractionContext>()
    .AddComponentInteractions<ModalSubmitInteraction, ModalInteractionContext>();

var host = builder.Build();

// Add component interactions using minimal APIs
host.AddComponentInteraction<ButtonInteractionContext>("ping", () => "Pong!");
host.AddComponentInteraction<StringMenuInteractionContext>("string", (StringMenuInteractionContext context) => string.Join("\n", context.Values));
host.AddComponentInteraction<UserMenuInteractionContext>("user", (UserMenuInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<RoleMenuInteractionContext>("role", (RoleMenuInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<MentionableMenuInteractionContext>("mentionable", (MentionableMenuInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<ChannelMenuInteractionContext>("channel", (ChannelMenuInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<ModalInteractionContext>("modal", (ModalInteractionContext context) => ((TextInputComponent)context.Components[0]).Value);

// Add component interactions from modules
host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
