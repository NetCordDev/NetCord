using System.Reflection;

using NetCord.Services.Commands;
using NetCord.Services.Interactions;
using NetCord.Services.SlashCommands;

namespace NetCord.Test;

internal static class Program
{
    private static readonly GatewayClient _client = new(Environment.GetEnvironmentVariable("token")!, TokenType.Bot, new() { Intents = GatewayIntent.All, Presence = new(UserStatusType.Invisible, true) });
    private static readonly CommandService _commandService = new();
    private static readonly InteractionService<ButtonInteractionContext> _buttonInteractionService = new();
    private static readonly InteractionService<MenuInteractionContext> _menuInteractionService = new();
    private static readonly InteractionService<ModalSubmitInteractionContext> _modalSubmitInteractionService = new();
    private static readonly SlashCommandService<SlashCommandContext> _slashCommandService;

    static Program()
    {
        SlashCommandServiceOptions<SlashCommandContext> options = new();
        options.TypeReaders.Add(typeof(Permission), new SlashCommands.PermissionTypeReader());
        _slashCommandService = new(options);
    }

    private static async Task Main()
    {
        _client.Log += Client_Log;
        _client.MessageReceived += Client_MessageReceived;
        _client.InteractionCreated += Client_InteractionCreated;
        var assembly = Assembly.GetEntryAssembly()!;
        _commandService.AddModules(assembly);
        _buttonInteractionService.AddModules(assembly);
        _menuInteractionService.AddModules(assembly);
        _modalSubmitInteractionService.AddModules(assembly);
        _slashCommandService.AddModules(assembly);
        await _client.StartAsync();
        await _client.ReadyAsync;
        await _slashCommandService.CreateCommandsAsync(_client.Rest, _client.Application!.Id, true);
        await Task.Delay(-1);
    }

    private static async void Client_InteractionCreated(Interaction interaction)
    {
        try
        {
            switch (interaction)
            {
                case ApplicationCommandInteraction applicationCommandInteraction:
                    await _slashCommandService.ExecuteAsync(new(applicationCommandInteraction, _client));
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

    private static async void Client_MessageReceived(Message message)
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

    private static void Client_Log(string text, LogType type)
    {
        Console.ForegroundColor = type == LogType.Gateway ? ConsoleColor.Cyan : ConsoleColor.DarkRed;
        Console.WriteLine($"{DateTime.Now:T} {type}\t{text}");
        Console.ResetColor();
    }
}