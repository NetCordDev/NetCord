
using NetCord.Commands;

namespace NetCord.Test
{
    public class CustomCommandContext : CommandContext
    {
        public string Prefix { get; }

        public CustomCommandContext(string prefix, UserMessage message, BotClient client) : base(message, client)
        {
            Prefix = prefix;
        }
    }
}
