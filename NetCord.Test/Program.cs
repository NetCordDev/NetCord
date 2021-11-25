using System.Reflection;

using NetCord.Commands;
using NetCord.Interactions;

namespace NetCord.Test;

internal static class Program
{
    private static readonly BotClient _client = new(Environment.GetEnvironmentVariable("token"), TokenType.Bot, new() { Intents = Intents.All });
    private static readonly CommandService _commandService = new();
    private static readonly InteractionService _interactionService = new();

    private async static Task Main()
    {
        _client.Log += Client_Log;
        _client.MessageReceived += Client_MessageReceived;
        _client.ButtonInteractionCreated += Client_ButtonInteractionCreated;
        _client.MenuInteractionCreated += Client_MenuInteractionCreated;
        var assembly = Assembly.GetEntryAssembly();
        _commandService.AddModules(assembly);
        _interactionService.AddModules(assembly);
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    private static async void Client_MenuInteractionCreated(MenuInteraction interaction)
    {
        try
        {
            await _interactionService.ExecuteAsync(new MenuInteractionContext(interaction, _client));
        }
        catch (Exception ex)
        {
            InteractionMessageBuilder message = new()
            {
                Content = ex.Message,
                Ephemeral = true
            };
            await interaction.EndWithReplyAsync(message.Build());
        }
    }

    private static async void Client_ButtonInteractionCreated(ButtonInteraction interaction)
    {
        try
        {
            await _interactionService.ExecuteAsync(new ButtonInteractionContext(interaction, _client));
        }
        catch (Exception ex)
        {
            InteractionMessageBuilder message = new()
            {
                Content = ex.Message,
                Ephemeral = true
            };
            await interaction.EndWithReplyAsync(message.Build());
        }
    }

    private static async void Client_MessageReceived(UserMessage message)
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
        if (type == LogType.Gateway)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now:T} {type}\t{text}");
            Console.ResetColor();
        }
        else if (type == LogType.Exception)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{DateTime.Now:T} {type}\t{text}");
            Console.ResetColor();
        }
    }
}