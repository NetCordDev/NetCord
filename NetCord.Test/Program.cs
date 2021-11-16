using System.Reflection;

using NetCord.Commands;
using NetCord.Interactions;

namespace NetCord.Test
{
    internal static class Program
    {
        private static BotClient _client;
        private static CommandService<CustomCommandContext> _commandService;
        private static InteractionService<CustomButtonInteractionContext, CustomMenuInteractionContext> _interactionService;

        private async static Task Main()
        {
            _client = new(Environment.GetEnvironmentVariable("token"), TokenType.Bot);
            _client.Log += Client_Log;
            _client.MessageReceived += Client_MessageReceived;
            _client.ButtonInteractionCreated += Client_ButtonInteractionCreated;
            _client.MenuInteractionCreated += Client_MenuInteractionCreated;
            _commandService = new();
            _commandService.AddModules(Assembly.GetEntryAssembly());
            _interactionService = new();
            _interactionService.AddModules(Assembly.GetEntryAssembly());
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private static async Task Client_MenuInteractionCreated(MenuInteraction interaction)
        {
            try
            {
                await _interactionService.ExecuteAsync(new CustomMenuInteractionContext(interaction, _client));
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
                await _interactionService.ExecuteAsync(new CustomButtonInteractionContext(interaction, _client));
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
                        await _commandService.ExecuteAsync(prefix.Length, new(prefix, message, _client));
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
}
