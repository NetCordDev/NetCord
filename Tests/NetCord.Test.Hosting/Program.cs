using Microsoft.Extensions.Hosting;

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
    .AddDiscordGateway(o => o.Configuration = new()
    {
        Intents = GatewayIntents.All,
    })
    .AddApplicationCommands<SlashCommandInteraction, SlashCommandContext, AutocompleteInteractionContext>()
    .AddApplicationCommands<UserCommandInteraction, UserCommandContext>()
    .AddApplicationCommands<MessageCommandInteraction, MessageCommandContext>()
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .AddCommands<CommandContext>()
    .AddGatewayEventHandlers(typeof(Program).Assembly);

var host = builder.Build()
    .AddSlashCommand<SlashCommandContext>("ping", "Ping!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s = "wzium") => $"Pong! {s}")
    .AddSlashCommand<SlashCommandContext>("help", "Help!", (ApplicationCommandService<SlashCommandContext> slashCommandService, SlashCommandContext context) => string.Join('\n', slashCommandService.GetCommands()!.Values.Select(c => c.Name)))
    .AddSlashCommand<SlashCommandContext>("button", "Button!", () =>
    {
        return new InteractionMessageProperties()
        {
            Components = [new ActionRowProperties([new ButtonProperties("button", "Button!", ButtonStyle.Primary)])],
        };
    })
    .AddSlashCommand<SlashCommandContext>("exception", "Exception!", (Action<string>)((string s) => throw new("Exception!")))
    .AddSlashCommand<SlashCommandContext>("exception-button", "Exception button!", () => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties("exception", "Exception!", ButtonStyle.Danger)])))
    .AddUserCommand<UserCommandContext>("ping", () => "Pong!")
    .AddMessageCommand<MessageCommandContext>("ping", () => "Pong!")
    .AddComponentInteraction<ButtonInteractionContext>("button", () => "Button!")
    .AddComponentInteraction<ButtonInteractionContext>("exception", (Action)(() => throw new("Exception!")))
    .AddCommand<CommandContext>(["ping"], () => "Pong!")
    .AddCommand<CommandContext>(["exception"], (Action)(() => throw new("Exception!")))
    //.AddModules(Assembly.GetEntryAssembly()!)
    .AddApplicationCommandModule<SlashCommandContext, SlashCommandModule>()
    .UseGatewayEventHandlers();

await host.RunAsync();
