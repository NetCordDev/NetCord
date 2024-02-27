using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .UseComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
    .UseComponentInteractions<UserMenuInteraction, UserMenuInteractionContext>()
    .UseComponentInteractions<RoleMenuInteraction, RoleMenuInteractionContext>()
    .UseComponentInteractions<MentionableMenuInteraction, MentionableMenuInteractionContext>()
    .UseComponentInteractions<ChannelMenuInteraction, ChannelMenuInteractionContext>()
    .UseComponentInteractions<ModalInteraction, ModalSubmitInteractionContext>();

var host = builder.Build()
    .AddComponentInteraction<ButtonInteractionContext>("ping", () => "Pong!")
    .AddComponentInteraction<StringMenuInteractionContext>("string", (StringMenuInteractionContext context) => string.Join("\n", context.SelectedValues))
    .AddComponentInteraction<UserMenuInteractionContext>("user", (UserMenuInteractionContext context) => string.Join("\n", context.SelectedUsers))
    .AddComponentInteraction<RoleMenuInteractionContext>("role", (RoleMenuInteractionContext context) => string.Join("\n", context.SelectedRoles))
    .AddComponentInteraction<MentionableMenuInteractionContext>("mentionable", (MentionableMenuInteractionContext context) => string.Join("\n", context.SelectedMentionables))
    .AddComponentInteraction<ChannelMenuInteractionContext>("channel", (ChannelMenuInteractionContext context) => string.Join("\n", context.SelectedChannels))
    .AddComponentInteraction<ModalSubmitInteractionContext>("modal", (ModalSubmitInteractionContext context) => context.Components[0].Value)
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
