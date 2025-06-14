using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services;
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
    .AddGatewayEventHandler<MessageReactionAddAndMessageDeleteHandler>()
    .AddSingleton("Wzium")
    .AddKeyedSingleton("key", "Wzium2");

var host = builder.Build()
    .AddSlashCommand("ping", "Ping!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s = "wzium") => $"Pong! {s}")
    .AddSlashCommand("help", "Help!", (ApplicationCommandService<ApplicationCommandContext> slashCommandService, ApplicationCommandContext context) => string.Join('\n', slashCommandService.GetCommands()!.Values.Select(c => c.Name)))
    .AddSlashCommand("keyed-di", "Test of keyed DI", ([FromKeyedServices("key")][Optional][DefaultParameterValue(null)] string? keyedWzium, string wzium, ApplicationCommandContext context) => $"{keyedWzium} {wzium}")
    .AddSlashCommand("button", "Button!", () =>
    {
        return new InteractionMessageProperties()
        {
            Components = [new ActionRowProperties([new ButtonProperties("button", "Button!", ButtonStyle.Primary)])],
        };
    })
    .AddSlashCommand("exception", "Exception!", (Action<string>)(s => throw new("Exception!")))
    .AddSlashCommand("exception-button", "Exception button!", () => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties("exception", "Exception!", ButtonStyle.Danger)])))
    .AddUserCommand("ping", () => "Pong!")
    .AddMessageCommand("Content", (RestMessage message) => message.Content)
    .AddUserCommand("Name", (GatewayClient client, ApplicationCommandContext context, User user) => user.Username)
    .AddMessageCommand("ping", () => "Pong!")
    .AddComponentInteraction<ButtonInteractionContext>("button", () => "Button!")
    .AddComponentInteraction<ButtonInteractionContext>("exception", (Action<IServiceProvider, ButtonInteractionContext>)((provider, context) => throw new("Exception!")))
    .AddCommand(["ping"], () => "Pong!")
    .AddCommand(["exception"], (Action)(() => throw new("Exception!")))
    .AddSlashCommand("menu", "Create a menu!", () => new InteractionMessageProperties().AddComponents(new StringMenuProperties("menu", [new StringMenuSelectOptionProperties("xd", "xd"), new StringMenuSelectOptionProperties("ad", "ad")])))
    .AddComponentInteraction<StringMenuInteractionContext>("menu", () => "XD")
    .AddApplicationCommandModule<ApplicationCommandModule>()
    .AddApplicationCommandModule<DITestModule>()
    .AddSlashCommand("yellow", "Yellow!", builder =>
    {
        builder.AddSubCommand("green", "Green!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)]
        (string wzium,
                                                  ApplicationCommandContext context,
                                                  [SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string value) => $"green {value}, wzium: {wzium}");
        builder.AddSubCommand("blue", "Blue!", () => "blue");
        builder.AddSubCommand("red", "Red!", builder =>
        {
            builder.AddSubCommand("orange", "Orange!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)] () => "orange");
            builder.AddSubCommand("purple", "Purple!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s) => $"purple {s}");
        });
    })
    .AddSlashCommand("context-accessor", "Context Accessor Test!", (IContextAccessor<ApplicationCommandContext> contextAccessor, ApplicationCommandContext context) =>
    {
        string? content;

        if (contextAccessor.Context is { } accessorContext)
        {
            content = (accessorContext == context).ToString();
        }
        else
            content = "Context is null.";

        return new InteractionMessageProperties()
            .WithContent(content)
            .AddComponents(new ActionRowProperties().AddButtons(new ButtonProperties("context-accessor", "Test", ButtonStyle.Primary)));
    })
    .AddComponentInteraction<ButtonInteractionContext>("context-accessor", (IContextAccessor<ButtonInteractionContext> contextAccessor, ButtonInteractionContext context) =>
    {
        string? content;

        if (contextAccessor.Context is { } accessorContext)
            content = (accessorContext == context).ToString();
        else
            content = "Context is null.";

        return content;
    })
    .AddCommand(["context-accessor"], (IContextAccessor<CommandContext> contextAccessor, CommandContext context) =>
    {
        string? content;

        if (contextAccessor.Context is { } accessorContext)
            content = (accessorContext == context).ToString();
        else
            content = "Context is null.";

        return new ReplyMessageProperties()
            .WithContent(content)
            .AddComponents(new ActionRowProperties().AddButtons(new ButtonProperties("context-accessor", "Test", ButtonStyle.Primary)));
    })
    .UseGatewayEventHandlers();

await host.RunAsync();
