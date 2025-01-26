using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;
using NetCord.Test.Hosting;

//var builder = Host.CreateDefaultBuilder(args)
//    .UseDiscordGateway(o => o.Configuration = new()
//    {
//        Intents = GatewayIntents.All,
//    })
//    .UseApplicationCommandService<SlashCommandInteraction, SlashCommandContext, AutocompleteInteractionContext>()
//    .UseApplicationCommandService<UserCommandInteraction, UserCommandContext>()
//    .UseApplicationCommandService<MessageCommandInteraction, MessageCommandContext>()
//    .UseInteractionService<ButtonInteraction, ButtonInteractionContext>()
//    .UseCommandService<CommandContext>();

//builder.ConfigureServices(services =>
//{
//    //services.AddGatewayEventHandler<MessageReactionAddHandler>();
//    //services.AddGatewayEventHandler<ConnectHandler>();
//    //services.AddGatewayEventHandler<ChannelCreateUpdateDeleteHandler>();
//    services.AddGatewayEventHandlers(typeof(Program).Assembly);
//});

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .ConfigureDiscordGateway(o => o.Presence = new(UserStatusType.DoNotDisturb))
    .ConfigureCommands<CommandContext>(o => o.Prefix = "!")
    .ConfigureCommands(o => o.Prefix = ">")
    .ConfigureApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>(o => o.DefaultParameterDescriptionFormat = "AA")
    .ConfigureApplicationCommands(o => o.DefaultParameterDescriptionFormat = "XD")
    .AddDiscordGateway(o => o.Intents = GatewayIntents.All)
    .AddApplicationCommands(options =>
    {
        options.ResultHandler = new CustomApplicationCommandResultHandler();
    })
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .AddComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
    .AddCommands()
    .AddGatewayEventHandler<Message>(nameof(GatewayClient.MessageCreate), (Message message, ILogger<Message> logger) => logger.LogInformation("Content: {}", message.Content))
    .AddGatewayEventHandler<ChannelCreateUpdateDeleteHandler>()
    .AddGatewayEventHandler<ConnectHandler>()
    .AddGatewayEventHandler<MessageReactionAddAndMessageDeleteHandler>();

var host = builder.Build()
    .AddSlashCommand("ping", "Ping!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s = "wzium") => $"Pong! {s}")
    .AddSlashCommand("help", "Help!", (ApplicationCommandService<ApplicationCommandContext> slashCommandService, ApplicationCommandContext context) => string.Join('\n', slashCommandService.GetCommands()!.Values.Select(c => c.Name)))
    .AddSlashCommand("button", "Button!", () =>
    {
        return new InteractionMessageProperties()
        {
            Components = [new ActionRowProperties([new ButtonProperties("button", "Button!", ButtonStyle.Primary)])],
        };
    })
    .AddSlashCommand("exception", "Exception!", (Action<string>)((string s) => throw new("Exception!")))
    .AddSlashCommand("exception-button", "Exception button!", () => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties("exception", "Exception!", ButtonStyle.Danger)])))
    .AddUserCommand("ping", () => "Pong!")
    .AddMessageCommand("Content", (RestMessage message) => message.Content)
    .AddUserCommand("Name", (GatewayClient client, ApplicationCommandContext context, User user) => user.Username)
    .AddMessageCommand("ping", () => "Pong!")
    .AddComponentInteraction<ButtonInteractionContext>("button", () => "Button!")
    .AddComponentInteraction<ButtonInteractionContext>("exception", (Action<IServiceProvider, ButtonInteractionContext>)((IServiceProvider provider, ButtonInteractionContext context) => throw new("Exception!")))
    .AddCommand(["ping"], () => "Pong!")
    .AddCommand(["exception"], (Action)(() => throw new("Exception!")))
    .AddSlashCommand("menu", "Create a menu!", () => new InteractionMessageProperties().AddComponents(new StringMenuProperties("menu", [new StringMenuSelectOptionProperties("xd", "xd"), new StringMenuSelectOptionProperties("ad", "ad")])))
    .AddComponentInteraction<StringMenuInteractionContext>("menu", () => "XD")
    .AddApplicationCommandModule<ApplicationCommandModule>()
    .UseGatewayEventHandlers();

await host.RunAsync();
