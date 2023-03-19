using System.Diagnostics;
using System.Reflection;

using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.Interactions;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!), new()
    {
        Intents = GatewayIntents.All,
        ConnectionProperties = ConnectionPropertiesProperties.IOS,
    });

    private static readonly CommandService<CommandContext> _commandService = new();
    private static readonly InteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly InteractionService<StringMenuInteractionContext> _stringMenuInteractionService = new();
    private static readonly InteractionService<UserMenuInteractionContext> _userMenuInteractionService = new();
    private static readonly InteractionService<RoleMenuInteractionContext> _roleMenuInteractionService = new();
    private static readonly InteractionService<MentionableMenuInteractionContext> _mentionableMenuInteractionService = new();
    private static readonly InteractionService<ChannelMenuInteractionContext> _channelMenuInteractionService = new();
    private static readonly InteractionService<ModalSubmitInteractionContext> _modalSubmitInteractionService = new();
    private static readonly ApplicationCommandService<SlashCommandContext, AutocompleteInteractionContext> _slashCommandService;
    private static readonly ApplicationCommandService<MessageCommandContext> _messageCommandService = new();
    private static readonly ApplicationCommandService<UserCommandContext> _userCommandService = new();

    private static readonly Dictionary<ulong, VoiceState> _voiceData = new();

    static Program()
    {
        ApplicationCommandServiceConfiguration<SlashCommandContext> options = new();
        options.TypeReaders.Add(typeof(Permissions), new SlashCommands.PermissionsTypeReader());
        _slashCommandService = new(options);
    }

    private static async Task Main()
    {
        _client.Log += Client_Log;
        _client.MessageCreate += Client_MessageCreate;
        _client.InteractionCreate += Client_InteractionCreate;
        _client.VoiceStateUpdate += Client_VoiceStateUpdate;
        _client.VoiceServerUpdate += Client_VoiceServerUpdate;
        var assembly = Assembly.GetEntryAssembly()!;
        _commandService.AddModules(assembly);
        _buttonInteractionService.AddModules(assembly);
        _stringMenuInteractionService.AddModules(assembly);
        _userMenuInteractionService.AddModules(assembly);
        _roleMenuInteractionService.AddModules(assembly);
        _mentionableMenuInteractionService.AddModules(assembly);
        _channelMenuInteractionService.AddModules(assembly);
        _modalSubmitInteractionService.AddModules(assembly);
        _slashCommandService.AddModules(assembly);
        _messageCommandService.AddModules(assembly);
        _userCommandService.AddModules(assembly);
        ApplicationCommandServiceManager manager = new();
        manager.AddService(_slashCommandService);
        manager.AddService(_messageCommandService);
        manager.AddService(_userCommandService);

        await _client.StartAsync();
        await _client.ReadyAsync;
        try
        {
            await manager.CreateCommandsAsync(_client.Rest, _client.ApplicationId!.Value, true);
        }
        catch (RestException ex)
        {
            Console.WriteLine(await ex.GetDiscordErrorMessageAsync());
        }
        await Task.Delay(-1);
    }

    private static ValueTask Client_VoiceStateUpdate(VoiceState arg)
    {
        _voiceData[arg.GuildId!.Value] = arg;
        return default;
    }

    private static async ValueTask Client_VoiceServerUpdate(VoiceServerUpdateEventArgs arg)
    {
        var state = _voiceData[arg.GuildId];
        VoiceClient client = new(state.UserId, state.SessionId, arg.Endpoint!, arg.GuildId, arg.Token, new()
        {
            RedirectInputStreams = true,
        });

        client.Log += (message) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
            return default;
        };

        await client.StartAsync();
        await client.ReadyAsync;

        var opusStream = client.CreateOutputStream(/*false*/);
        var stream = new OpusEncodeStream(opusStream, VoiceChannels.Stereo, OpusApplication.Audio);

        await client.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        var url = "https://filesamples.com/samples/audio/mp3/Symphony%20No.6%20(1st%20movement).mp3"; // 00:12:08
        //var url = "https://file-examples.com/storage/fef1706276640fa2f99a5a4/2017/11/file_example_MP3_700KB.mp3"; // 00:00:27
        var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{url}\" -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
        })!;

        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        await stream.FlushAsync();

        //client.VoiceReceive += args => opusStream.WriteAsync(args.Frame);
        await Task.Delay(-1);
    }

    private static async ValueTask Client_InteractionCreate(Interaction interaction)
    {
        try
        {
            await (interaction switch
            {
                SlashCommandInteraction slashCommandInteraction => _slashCommandService.ExecuteAsync(new(slashCommandInteraction, _client)),
                MessageCommandInteraction messageCommandInteraction => _messageCommandService.ExecuteAsync(new(messageCommandInteraction, _client)),
                UserCommandInteraction userCommandInteraction => _userCommandService.ExecuteAsync(new(userCommandInteraction, _client)),
                StringMenuInteraction stringMenuInteraction => _stringMenuInteractionService.ExecuteAsync(new(stringMenuInteraction, _client)),
                UserMenuInteraction userMenuInteraction => _userMenuInteractionService.ExecuteAsync(new(userMenuInteraction, _client)),
                RoleMenuInteraction roleMenuInteraction => _roleMenuInteractionService.ExecuteAsync(new(roleMenuInteraction, _client)),
                MentionableMenuInteraction mentionableMenuInteraction => _mentionableMenuInteractionService.ExecuteAsync(new(mentionableMenuInteraction, _client)),
                ChannelMenuInteraction channelMenuInteraction => _channelMenuInteractionService.ExecuteAsync(new(channelMenuInteraction, _client)),
                ButtonInteraction buttonInteraction => _buttonInteractionService.ExecuteAsync(new(buttonInteraction, _client)),
                ApplicationCommandAutocompleteInteraction applicationCommandAutocompleteInteraction => _slashCommandService.ExecuteAutocompleteAsync(new(applicationCommandAutocompleteInteraction, _client)),
                ModalSubmitInteraction modalSubmitInteraction => _modalSubmitInteractionService.ExecuteAsync(new(modalSubmitInteraction, _client)),
                _ => throw new("Invalid interaction"),
            });
        }
        catch (Exception ex)
        {
            InteractionMessageProperties message = new()
            {
                Content = ex.Message,
                Flags = MessageFlags.Ephemeral
            };
            try
            {
                await interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(message));
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
                try
                {
                    await _commandService.ExecuteAsync(prefix.Length, new(message, _client));
                }
                catch (Exception ex)
                {
                    try
                    {
                        await message.ReplyAsync(ex.Message, failIfNotExists: false);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    private static ValueTask Client_Log(LogMessage message)
    {
        Console.ForegroundColor = message.Severity == LogSeverity.Info ? ConsoleColor.Cyan : ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.ResetColor();
        return default;
    }
}
