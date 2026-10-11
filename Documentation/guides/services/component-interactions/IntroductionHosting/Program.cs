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
    .AddComponentInteractions<StringSelectInteraction, StringSelectInteractionContext>()
    .AddComponentInteractions<UserSelectInteraction, UserSelectInteractionContext>()
    .AddComponentInteractions<RoleSelectInteraction, RoleSelectInteractionContext>()
    .AddComponentInteractions<MentionableSelectInteraction, MentionableSelectInteractionContext>()
    .AddComponentInteractions<ChannelSelectInteraction, ChannelSelectInteractionContext>()
    .AddComponentInteractions<ModalSubmitInteraction, ModalInteractionContext>();

var host = builder.Build();

// Add component interactions using minimal APIs
host.AddComponentInteraction<ButtonInteractionContext>("ping", () => "Pong!");
host.AddComponentInteraction<StringSelectInteractionContext>("string", (StringSelectInteractionContext context) => string.Join("\n", context.Values));
host.AddComponentInteraction<UserSelectInteractionContext>("user", (UserSelectInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<RoleSelectInteractionContext>("role", (RoleSelectInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<MentionableSelectInteractionContext>("mentionable", (MentionableSelectInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<ChannelSelectInteractionContext>("channel", (ChannelSelectInteractionContext context) => string.Join("\n", context.SelectedValues));
host.AddComponentInteraction<ModalInteractionContext>("modal", (ModalInteractionContext context) => ((TextInputComponent)context.Components[0]).Value);

// Add component interactions from modules
host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
