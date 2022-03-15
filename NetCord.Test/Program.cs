using System.Reflection;

using NetCord.Services.Commands;
using NetCord.Services.Interactions;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(Environment.GetEnvironmentVariable("token")!, TokenType.Bot, new() { Intents = GatewayIntent.All, Presence = new(UserStatusType.Invisible, true), Shard = new(0, 1) });
    private static readonly CommandService<CommandContext> _commandService = new();
    private static readonly InteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly InteractionService<MenuInteractionContext> _menuInteractionService = new();
    private static readonly InteractionService<ModalSubmitInteractionContext> _modalSubmitInteractionService = new();
    private static readonly ApplicationCommandService<SlashCommandContext> _slashCommandService;
    private static readonly ApplicationCommandService<MessageCommandContext> _messageCommandService = new();
    private static readonly ApplicationCommandService<UserCommandContext> _userCommandService = new();

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
        manager.AddApplicationCommandService(_slashCommandService);
        manager.AddApplicationCommandService(_messageCommandService);
        manager.AddApplicationCommandService(_userCommandService);
        await manager.CreateCommandsAsync(_client.Rest, _client.ApplicationId!.Value, true);
        await Task.Delay(-1);
    }

    private static async Task Client_InteractionCreated(Interaction interaction)
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

    private static async Task Client_MessageReceived(Message message)
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

    private static Task Client_Log(LogMessage message)
    {
        Console.ForegroundColor = message.Severity == LogSeverity.Info ? ConsoleColor.Cyan : ConsoleColor.DarkRed;
        Console.WriteLine(message);
        Console.ResetColor();
        return Task.CompletedTask;
    }
}