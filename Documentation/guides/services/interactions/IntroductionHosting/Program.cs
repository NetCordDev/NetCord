using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Interactions;
using NetCord.Services.Interactions;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseInteractionService<ButtonInteraction, ButtonInteractionContext>()
    .UseInteractionService<StringMenuInteraction, StringMenuInteractionContext>()
    .UseInteractionService<UserMenuInteraction, UserMenuInteractionContext>()
    .UseInteractionService<RoleMenuInteraction, RoleMenuInteractionContext>()
    .UseInteractionService<MentionableMenuInteraction, MentionableMenuInteractionContext>()
    .UseInteractionService<ChannelMenuInteraction, ChannelMenuInteractionContext>()
    .UseInteractionService<ModalSubmitInteraction, ModalSubmitInteractionContext>();

var host = builder.Build()
    .AddInteraction<ButtonInteractionContext>("ping", () => "Pong!")
    .AddInteraction<StringMenuInteractionContext>("string", (StringMenuInteractionContext context) => string.Join("\n", context.SelectedValues))
    .AddInteraction<UserMenuInteractionContext>("user", (UserMenuInteractionContext context) => string.Join("\n", context.SelectedUsers))
    .AddInteraction<RoleMenuInteractionContext>("role", (RoleMenuInteractionContext context) => string.Join("\n", context.SelectedRoles))
    .AddInteraction<MentionableMenuInteractionContext>("mentionable", (MentionableMenuInteractionContext context) => string.Join("\n", context.SelectedMentionables))
    .AddInteraction<ChannelMenuInteractionContext>("channel", (ChannelMenuInteractionContext context) => string.Join("\n", context.SelectedChannels))
    .AddInteraction<ModalSubmitInteractionContext>("modal", (ModalSubmitInteractionContext context) => string.Join("\n", context.Values))
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
