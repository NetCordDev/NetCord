using System.Diagnostics;
using System.Reflection;

using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.Interactions;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!), new() { Intents = GatewayIntent.All, ConnectionProperties = ConnectionPropertiesProperties.IOS });
    private static readonly CommandService<CommandContext> _commandService = new();
    private static readonly InteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly InteractionService<MenuInteractionContext> _menuInteractionService = new();
    private static readonly InteractionService<ModalSubmitInteractionContext> _modalSubmitInteractionService = new();
    private static readonly ApplicationCommandService<SlashCommandContext> _slashCommandService;
    private static readonly ApplicationCommandService<MessageCommandContext> _messageCommandService = new();
    private static readonly ApplicationCommandService<UserCommandContext> _userCommandService = new();

    private static readonly Dictionary<Snowflake, VoiceState> _voiceData = new();

    static Program()
    {
        ApplicationCommandServiceOptions<SlashCommandContext> options = new();
        options.TypeReaders.Add(typeof(Permission), new SlashCommands.PermissionTypeReader());
        _slashCommandService = new(options);
    }

    private static async Task Main()
    {
        _client.Log += Client_Log;
        _client.MessageCreate += Client_MessageReceived;
        _client.InteractionCreate += Client_InteractionCreated;
        _client.VoiceStateUpdate += Client_VoiceStateUpdate;
        _client.VoiceServerUpdate += Client_VoiceServerUpdate;
        var assembly = Assembly.GetEntryAssembly()!;
        _commandService.AddModules(assembly);
        _buttonInteractionService.AddModules(assembly);
        _menuInteractionService.AddModules(assembly);
        _modalSubmitInteractionService.AddModules(assembly);
        _slashCommandService.AddModules(assembly);
        _messageCommandService.AddModules(assembly);
        _userCommandService.AddModules(assembly);
        await _client.StartAsync();
        await _client.ReadyAsync;
        ApplicationCommandServiceManager manager = new();
        manager.AddService(_slashCommandService);
        manager.AddService(_messageCommandService);
        manager.AddService(_userCommandService);
        await manager.CreateCommandsAsync(_client.Rest, _client.ApplicationId!.Value, true);
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
        Gateway.Voice.VoiceClient client = new(arg.Endpoint!, arg.GuildId, state.UserId, state.SessionId, arg.Token, new()
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
        var stream = client.CreatePCMStream(Gateway.Voice.OpusApplication.Audio);
        await client.ReadyAsync;
        await client.EnterSpeakingStateAsync(Gateway.Voice.SpeakingFlags.Microphone);
        var url = "https://cdn.discordapp.com/attachments/864636357821726730/982394204011520020/Pew_Pew-DKnight556-1379997159.mp3"; //00:00:02
        //var url = "https://filesamples.com/samples/audio/mp3/Symphony%20No.6%20(1st%20movement).mp3"; //00:12:08
        var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{url}\" -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
        })!;

        //client.VoiceReceive += (ssrc, frame) =>
        //{
        //    stream.Write(frame.Span);
        //    return default;
        //};
        try
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        }
        finally
        {
            await stream.FlushAsync();
        }
        //await _client.UpdateVoiceStateAsync(new VoiceStateProperties(arg.GuildId, null));
        await Task.Delay(-1);
    }

    private static async ValueTask Client_InteractionCreated(Interaction interaction)
    {
        try
        {
            switch (interaction)
            {
                case SlashCommandInteraction slashCommandInteraction:
                    await _slashCommandService.ExecuteAsync(new(slashCommandInteraction, _client));
                    break;
                case MessageCommandInteraction messageCommandInteraction:
                    await _messageCommandService.ExecuteAsync(new(messageCommandInteraction, _client));
                    break;
                case UserCommandInteraction userCommandInteraction:
                    await _userCommandService.ExecuteAsync(new(userCommandInteraction, _client));
                    break;
                case MenuInteraction menuInteraction:
                    await _menuInteractionService.ExecuteAsync(new(menuInteraction, _client));
                    break;
                case ButtonInteraction buttonInteraction:
                    await _buttonInteractionService.ExecuteAsync(new(buttonInteraction, _client));
                    break;
                case ApplicationCommandAutocompleteInteraction applicationCommandAutocompleteInteraction:
                    await _slashCommandService.ExecuteAutocompleteAsync(applicationCommandAutocompleteInteraction);
                    break;
                case ModalSubmitInteraction modalSubmitInteraction:
                    await _modalSubmitInteractionService.ExecuteAsync(new(modalSubmitInteraction, _client));
                    break;
            }
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

    private static async ValueTask Client_MessageReceived(Message message)
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
                    await message.ReplyAsync(ex.Message, failIfNotExists: false);
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