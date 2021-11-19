using System.Reflection;

using NetCord.Commands;
using NetCord.Interactions;

namespace NetCord.Test;

internal static class Program
{
    private static readonly BotClient _client = new(Environment.GetEnvironmentVariable("token"), TokenType.Bot);
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

    private static async Task Client_MenuInteractionCreated(MenuInteraction interaction)
    {
        try
        {
            await _interactionService.ExecuteAsync(new MenuInteractionContext(interaction, _client));
        }
        catch (Exception ex)
        {
            await ChannelHelper.SendMessageAsync(_client, ex.Message, interaction.ChannelId);
        }
    }

    private static async Task Client_ButtonInteractionCreated(ButtonInteraction interaction)
    {
        try
        {
            await _interactionService.ExecuteAsync(new ButtonInteractionContext(interaction, _client));
        }
        catch (Exception ex)
        {
            await ChannelHelper.SendMessageAsync(_client, ex.Message, interaction.ChannelId);
        }
    }

    private async static Task Client_MessageReceived(UserMessage message)
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
                    await ChannelHelper.SendMessageAsync(_client, ex.Message, message.ChannelId);
                }
            }
        }
    }

    private static Task Client_Log(string text, LogType type)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{DateTime.Now:T} {type}\t{text}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
}