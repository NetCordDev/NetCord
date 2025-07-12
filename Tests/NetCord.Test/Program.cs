using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;
using NetCord.Test.ApplicationCommands;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(new BotToken(Environment.GetEnvironmentVariable("token")!), new()
    {
        Intents = GatewayIntents.All,
        ConnectionProperties = ConnectionPropertiesProperties.IOS,
        Logger = new ConsoleLogger(LogLevel.Debug),
        Presence = new(UserStatusType.DoNotDisturb)
        {
            Activities = [new("Custom Status", UserActivityType.Custom) { State = "XD" }],
        }
        //Compression = new ZstandardGatewayCompression(),
        //Compression = new ZLibGatewayCompression(),
        //Compression = new UncompressedGatewayCompression(),
    });

    private static readonly CommandService<CommandContext> _commandService = new();
    private static readonly ComponentInteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly ComponentInteractionService<StringMenuInteractionContext> _stringMenuInteractionService = new();
    private static readonly ComponentInteractionService<UserMenuInteractionContext> _userMenuInteractionService = new();
    private static readonly ComponentInteractionService<RoleMenuInteractionContext> _roleMenuInteractionService = new();
    private static readonly ComponentInteractionService<MentionableMenuInteractionContext> _mentionableMenuInteractionService = new();
    private static readonly ComponentInteractionService<ChannelMenuInteractionContext> _channelMenuInteractionService = new();
    private static readonly ComponentInteractionService<ModalInteractionContext> _modalInteractionService = new();
    private static readonly ApplicationCommandService<SlashCommandContext, AutocompleteInteractionContext> _slashCommandService;
    private static readonly ApplicationCommandService<MessageCommandContext> _messageCommandService = new();
    private static readonly ApplicationCommandService<UserCommandContext> _userCommandService = new();
    private static readonly ApplicationCommandService<EntryPointCommandContext> _entryPointCommandService = new();

    private static readonly ServiceProvider _serviceProvider;

    static Program()
    {
        var configuration = ApplicationCommandServiceConfiguration<SlashCommandContext>.Default;
        configuration = configuration with
        {
            TypeReaders = configuration.TypeReaders.Add(typeof(Permissions), new PermissionsTypeReader()),
            ParameterNameProcessor = SnakeCaseSlashCommandParameterNameProcessor<SlashCommandContext>.Instance,
            LocalizationsProvider = new JsonLocalizationsProvider(new() { FileNameFormat = "localization.*.*.*.json" }),
            DefaultIntegrationTypes = [ApplicationIntegrationType.GuildInstall, ApplicationIntegrationType.UserInstall],
            //Storage = new NameApplicationCommandServiceStorage<SlashCommandContext>(),
        };
        _slashCommandService = new(configuration);

        ServiceCollection services = new();
        services.AddSingleton("wzium");
        services.AddKeyedSingleton("key", "wzium2");
        services.AddSingleton(new HttpClient());
        services.AddSingleton(new Dictionary<ulong, SemaphoreSlim>());
        _serviceProvider = services.BuildServiceProvider();
    }

    private static async Task Main()
    {
        _client.MessageCreate += Client_MessageCreate;
        _client.InteractionCreate += Client_InteractionCreate;
        _client.GuildAuditLogEntryCreate += Client_GuildAuditLogEntryCreate;

        var assembly = Assembly.GetEntryAssembly()!;
        _commandService.AddCommand(["pol"], ([Optional] object? o, CommandContext context) => "xd");
        _commandService.AddModules(assembly);

        _buttonInteractionService.AddModules(assembly);
        _buttonInteractionService.AddInteraction("wziummm", (ButtonInteractionContext context) => "wzium");
        _stringMenuInteractionService.AddModules(assembly);
        _userMenuInteractionService.AddModules(assembly);
        _roleMenuInteractionService.AddModules(assembly);
        _mentionableMenuInteractionService.AddModules(assembly);
        _channelMenuInteractionService.AddModules(assembly);
        _modalInteractionService.AddModules(assembly);
        _slashCommandService.AddSlashCommand("ping", "Ping!", (SlashCommandContext context, string s) => s);
        _slashCommandService.AddSlashCommand("keyed-di", "Test of keyed DI", ([FromKeyedServices("key")] string keyedWzium, string wzium, SlashCommandContext context) => $"{keyedWzium} {wzium}");

        _slashCommandService.AddSlashCommand("yellow", "Yellow!", builder =>
        {
            builder.AddSubCommand("green", "Green!", [RequireContext<SlashCommandContext>(RequiredContext.DM)]
            (string wzium,
                                                      SlashCommandContext context,
                                                      [SlashCommandParameter(AutocompleteProviderType = typeof(DDGAutocomplete))] string value) => $"green {value}, wzium: {wzium}");
            builder.AddSubCommand("blue", "Blue!", () => "blue");
            builder.AddSubCommand("red", "Red!", builder =>
            {
                builder.AddSubCommand("orange", "Orange!", [RequireContext<SlashCommandContext>(RequiredContext.DM)] () => "orange");
                builder.AddSubCommand("purple", "Purple!", async (SlashCommandContext context, [SlashCommandParameter(AutocompleteProviderType = typeof(DDGAutocomplete))] string s) =>
                {
                    var response = await context.Interaction.SendResponseAsync(InteractionCallback.LaunchActivity, true);
                    await context.Interaction.SendFollowupMessageAsync(response?.Interaction.ActivityInstanceId ?? "No activity instance ID");
                });
            });
        });

        _slashCommandService.AddSlashCommand("response-test", "Response Test!", async (SlashCommandContext context) =>
        {
            var response = await context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(), true);
            await context.Interaction.SendFollowupMessageAsync(response!.Interaction.ResponseMessageId.GetValueOrDefault().ToString());
        });

        _slashCommandService.AddModules(assembly);
        _messageCommandService.AddModules(assembly);

        _messageCommandService.AddMessageCommand("wziummm", InteractionMessageProperties (MessageCommandContext context) => new() { Components = [new ActionRowProperties([new ButtonProperties("wziummm", "WZIUM", ButtonStyle.Success)])] });

        _userCommandService.AddModules(assembly);

        _userCommandService.AddUserCommand("wziummm", (UserCommandContext context) => "wzium");

        _entryPointCommandService.AddEntryPointCommand("launch-xd", "LOL", (EntryPointCommandContext context) =>
        {
            return InteractionCallback.LaunchActivity;
        });

        await _client.StartAsync();

        await RegisterCommandsAsync(false);

        await Task.Delay(-1);
    }

    private static async ValueTask RegisterCommandsAsync(bool globally)
    {
        ApplicationCommandServiceManager manager = new();

        manager.AddService(_slashCommandService);
        manager.AddService(_messageCommandService);
        manager.AddService(_userCommandService);

        if (globally)
            manager.AddService(_entryPointCommandService);

        var client = _client.Rest;
        var id = _client.Id;
        ulong guildId = 856183259972763669;

        try
        {
            await manager.RegisterCommandsAsync(client, id, globally ? null : guildId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        if (globally)
            await client.BulkOverwriteGuildApplicationCommandsAsync(id, guildId, []);
        else
            await _entryPointCommandService.RegisterCommandsAsync(client, id);
    }

    private static async ValueTask Client_GuildAuditLogEntryCreate(AuditLogEntry entry)
    {
        if (entry.ActionType is AuditLogEvent.ChannelUpdate)
        {
            if (entry.TryGetChange<JsonChannel, string>(c => c.Name, out var change))
                await _client.Rest.SendMessageAsync(entry.TargetId!.Value, $"old: {change.OldValue} new: {change.NewValue}");
            else
                await _client.Rest.SendMessageAsync(entry.TargetId!.Value, "Name hasn't changed");
        }
        else if (entry.ActionType is AuditLogEvent.GuildUserUpdate)
        {
            var channel = _client.Cache.Guilds[entry.GuildId].Channels.Values.OfType<TextChannel>().First();

            if (entry.TryGetChange<JsonGuildUser, DateTimeOffset?>(u => u.TimeOutUntil, out var change))
                await channel.SendMessageAsync($"old: {change.OldValue} new: {change.NewValue}");
            else
                await channel.SendMessageAsync("Time out hasn't changed");
        }
    }

    private static async ValueTask Client_InteractionCreate(Interaction interaction)
    {
        var result = await (interaction switch
        {
            SlashCommandInteraction slashCommandInteraction => _slashCommandService.ExecuteAsync(new(slashCommandInteraction, _client), _serviceProvider),
            MessageCommandInteraction messageCommandInteraction => _messageCommandService.ExecuteAsync(new(messageCommandInteraction, _client), _serviceProvider),
            UserCommandInteraction userCommandInteraction => _userCommandService.ExecuteAsync(new(userCommandInteraction, _client), _serviceProvider),
            EntryPointCommandInteraction entryPointCommandInteraction => _entryPointCommandService.ExecuteAsync(new(entryPointCommandInteraction, _client), _serviceProvider),
            StringMenuInteraction stringMenuInteraction => _stringMenuInteractionService.ExecuteAsync(new(stringMenuInteraction, _client), _serviceProvider),
            UserMenuInteraction userMenuInteraction => _userMenuInteractionService.ExecuteAsync(new(userMenuInteraction, _client), _serviceProvider),
            RoleMenuInteraction roleMenuInteraction => _roleMenuInteractionService.ExecuteAsync(new(roleMenuInteraction, _client), _serviceProvider),
            MentionableMenuInteraction mentionableMenuInteraction => _mentionableMenuInteractionService.ExecuteAsync(new(mentionableMenuInteraction, _client), _serviceProvider),
            ChannelMenuInteraction channelMenuInteraction => _channelMenuInteractionService.ExecuteAsync(new(channelMenuInteraction, _client), _serviceProvider),
            ButtonInteraction buttonInteraction => _buttonInteractionService.ExecuteAsync(new(buttonInteraction, _client), _serviceProvider),
            AutocompleteInteraction autocompleteInteraction => _slashCommandService.ExecuteAutocompleteAsync(new(autocompleteInteraction, _client), _serviceProvider),
            ModalInteraction modalInteraction => _modalInteractionService.ExecuteAsync(new(modalInteraction, _client), _serviceProvider),
            _ => throw new("Invalid interaction."),
        });
        if (result is IFailResult failResult)
        {
            InteractionMessageProperties message = new()
            {
                Content = failResult.Message,
                Flags = MessageFlags.Ephemeral,
            };
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(message));
            }
            catch
            {
                try
                {
                    await interaction.SendFollowupMessageAsync(message);
                }
                catch
                {
                }
            }
        }
    }

    private static async ValueTask Client_MessageCreate(Message message)
    {
        if (!message.Author.IsBot)
        {
            const string prefix = "!";
            if (message.Content.StartsWith(prefix))
            {
                var result = await _commandService.ExecuteAsync(prefix.Length, new(message, _client), _serviceProvider);
                if (result is IFailResult failResult)
                {
                    try
                    {
                        await message.ReplyAsync(new()
                        {
                            Content = failResult.Message,
                            FailIfNotExists = false,
                        });
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
