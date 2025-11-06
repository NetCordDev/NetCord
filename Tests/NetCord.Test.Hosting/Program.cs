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
//    //services.AddGatewayHandler<MessageReactionAddHandler>();
//    //services.AddGatewayHandler<ConnectHandler>();
//    //services.AddGatewayHandler<ChannelCreateUpdateDeleteHandler>();
//    services.AddGatewayHandlers(typeof(Program).Assembly);
//});

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSingleton<HttpClient>()
    .AddGatewayHandler(GatewayEvent.InteractionCreate, (Interaction interaction) => Console.WriteLine(interaction))
    .ConfigureDiscordGateway(o => o.Presence = new(UserStatusType.DoNotDisturb))
    .ConfigureCommands<CommandContext>(o => o.Prefix = "!")
    .ConfigureCommands(o => o.Prefix = ">")
    .ConfigureApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>(o => o.DefaultParameterDescriptionFormat = "AA")
    .ConfigureApplicationCommands(o => o.DefaultParameterDescriptionFormat = "XD")
    .ConfigureApplicationCommands(o => o.AutoRegisterCommands = true)
    .AddDiscordGateway(o =>
    {
        o.Intents = GatewayIntents.All;
        o.AutoStartStop = true;
    })
    .AddApplicationCommands(options =>
    {
        options.ResultHandler = new CustomApplicationCommandResultHandler();
    })
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .AddComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
    .AddComponentInteractions<ModalInteraction, ModalInteractionContext>()
    .AddCommands()
    .AddGatewayHandler(GatewayEvent.MessageCreate, (Message message, ILogger<Message> logger) => logger.LogInformation("Content: {}", message.Content))
    .AddGatewayHandler<ChannelCreateUpdateDeleteHandler>()
    .AddGatewayHandler<ConnectHandler>()
    .AddGatewayHandler<MessageReactionAddAndMessageDeleteHandler>()
    .AddSingleton("Wzium")
    .AddKeyedSingleton("key", "Wzium2");

var host = builder.Build();

host.AddSlashCommand("ping", "Ping!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s = "wzium") => $"Pong! {s}");
host.AddSlashCommand("help", "Help!", (ApplicationCommandService<ApplicationCommandContext> slashCommandService, ApplicationCommandContext context) => string.Join('\n', slashCommandService.GetCommands().Select(c => c.Name)));
host.AddSlashCommand("keyed-di", "Test of keyed DI", ([FromKeyedServices("key")][Optional][DefaultParameterValue(null)] string? keyedWzium, string wzium, ApplicationCommandContext context) => $"{keyedWzium} {wzium}");
host.AddSlashCommand("button", "Button!", () =>
{
    return new InteractionMessageProperties()
    {
        Components = [new ActionRowProperties([new ButtonProperties("button", "Button!", ButtonStyle.Primary)])],
    };
});
host.AddSlashCommand("exception", "Exception!", (Action<string>)(s => throw new("Exception!")));
host.AddSlashCommand("exception-button", "Exception button!", () => new InteractionMessageProperties().AddComponents(new ActionRowProperties([new ButtonProperties("exception", "Exception!", ButtonStyle.Danger)])));
host.AddUserCommand("ping", () => "Pong!");
host.AddMessageCommand("Content", (RestMessage message) => message.Content);
host.AddUserCommand("Name", (GatewayClient client, ApplicationCommandContext context, User user) => user.Username);
host.AddMessageCommand("ping", () => "Pong!");
host.AddComponentInteraction<ButtonInteractionContext>("button", () => "Button!");
host.AddComponentInteraction<ButtonInteractionContext>("exception", (Action<IServiceProvider, ButtonInteractionContext>)((provider, context) => throw new("Exception!")));
host.AddCommand(["ping"], () => "Pong!");
host.AddCommand(["exception"], (Action)(() => throw new("Exception!")));
host.AddSlashCommand("menu", "Create a menu!", () => new InteractionMessageProperties().AddComponents(new StringMenuProperties("menu", [new StringMenuSelectOptionProperties("xd", "xd"), new StringMenuSelectOptionProperties("ad", "ad")])));
host.AddComponentInteraction<StringMenuInteractionContext>("menu", () => "XD");
host.AddApplicationCommandModule<ApplicationCommandModule>();
host.AddApplicationCommandModule<DITestModule>();

host.AddSlashCommandGroup("yellow", "Yellow!", builder =>
{
    builder.AddSubCommand("green", "Green!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)]
    (string wzium,
                                              ApplicationCommandContext context,
                                              [SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string value) => $"green {value}, wzium: {wzium}");

    builder.AddSubCommand("blue", "Blue!", () => "blue");

    builder.AddSubCommandGroup("red", "Red!", builder =>
    {
        builder.AddSubCommand("orange", "Orange!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)] () => "orange");
        builder.AddSubCommand("purple", "Purple!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s) => $"purple {s}");
    });
});

//var yellowGroup = host.AddSlashCommandGroup("yellow", "Yellow!");

//yellowGroup.AddSubCommand("green", "Green!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)]
//                                             (string wzium,
//                                              ApplicationCommandContext context,
//                                              [SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string value) => $"green {value}, wzium: {wzium}");

//yellowGroup.AddSubCommand("blue", "Blue!", () => "blue");

//var redYellowGroup = yellowGroup.AddSubCommandGroup("red", "Red!");

//redYellowGroup.AddSubCommand("orange", "Orange!", [RequireContext<ApplicationCommandContext>(RequiredContext.DM)] () => "orange");

//redYellowGroup.AddSubCommand("purple", "Purple!", ([SlashCommandParameter(AutocompleteProviderType = typeof(StringAutocompleteProvider))] string s) => $"purple {s}");

host.AddCommandGroup(["yellow"], builder =>
{
    builder.AddSubCommand(["green"], [RequireContext<CommandContext>(RequiredContext.DM)]
    (string wzium,
                                      CommandContext context,
                                      string value) => $"green {value}, wzium: {wzium}");

    builder.AddSubCommand(["blue"], () => "blue");

    builder.AddSubCommandGroup(["red"], builder =>
    {
        builder.AddSubCommand(["orange"], [RequireContext<CommandContext>(RequiredContext.DM)] () => "orange");
        builder.AddSubCommand(["purple"], (string s) => $"purple {s}");
    });
});

host.AddSlashCommand("context-accessor", "Context Accessor Test!", (IContextAccessor<ApplicationCommandContext> contextAccessor, ApplicationCommandContext context, [SlashCommandParameter(AutocompleteProviderType = typeof(ContextAccessorAutocompleteProvider))] string s) =>
{
    string? content;

    if (contextAccessor.Context is { } accessorContext)
    {
        content = $"{accessorContext == context} {s}";
    }
    else
        content = $"Context is null. {s}";

    return new InteractionMessageProperties()
        .WithContent(content)
        .AddComponents(new ActionRowProperties().AddComponents(new ButtonProperties("context-accessor", "Test", ButtonStyle.Primary)));
});

host.AddComponentInteraction<ButtonInteractionContext>("context-accessor", (IContextAccessor<ButtonInteractionContext> contextAccessor, ButtonInteractionContext context) =>
{
    string? content;

    if (contextAccessor.Context is { } accessorContext)
        content = (accessorContext == context).ToString();
    else
        content = "Context is null.";

    return content;
});

host.AddCommand(["context-accessor"], (IContextAccessor<CommandContext> contextAccessor, CommandContext context) =>
{
    string? content;

    if (contextAccessor.Context is { } accessorContext)
        content = (accessorContext == context).ToString();
    else
        content = "Context is null.";

    return new ReplyMessageProperties()
        .WithContent(content)
        .AddComponents(new ActionRowProperties().AddComponents(new ButtonProperties("context-accessor", "Test", ButtonStyle.Primary)));
});

host.AddSlashCommand("modal", "Modal", () =>
{
    return new ModalProperties("modal", "Modal")
    {
        new LabelProperties("Mentionable", new MentionableMenuProperties("mentionable")),
        new TextDisplayProperties("""
            ```cs
            Console.WriteLine("Wzium");
            ```
            """),
        new LabelProperties("User", new UserMenuProperties("user")),
        new LabelProperties("Channel", new ChannelMenuProperties("channel")),
        new LabelProperties("Role", new RoleMenuProperties("role")),
    };
});

host.AddSlashCommand("file-upload", "File Upload!", () =>
{
    return new ModalProperties("file upload", "File Upload")
    {
        new LabelProperties("Upload", new FileUploadProperties("xd").WithRequired(false).WithMinValues(2).WithMaxValues(3)),
    };
});

host.AddComponentInteraction<ModalInteractionContext>("file upload", async (HttpClient client, ModalInteractionContext context) =>
{
    var attachments = context.Components.OfType<Label>()
                                        .Select(l => l.Component)
                                        .OfType<FileUpload>()
                                        .SelectMany(u => u.Attachments)
                                        .ToArray();

    return new InteractionMessageProperties()
    {
        Flags = MessageFlags.IsComponentsV2 | MessageFlags.Ephemeral,
        Attachments = await Task.WhenAll(attachments.Select(async a => new AttachmentProperties(a.FileName, await client.GetStreamAsync(a.Url)))),
        Components = attachments.Select(a => (IMessageComponentProperties)new FileDisplayProperties(new($"attachment://{a.FileName}")))
                                .Prepend(new TextDisplayProperties("Uploaded files:"))
    };
});

host.AddComponentInteraction<ModalInteractionContext>("modal", (ModalInteractionContext context) => new InteractionMessageProperties().WithContent("a").WithFlags(MessageFlags.Ephemeral));

await host.RunAsync();
