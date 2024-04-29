﻿using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("mention-everyone", "Mentions @everyone",
        DefaultGuildUserPermissions = Permissions.MentionEveryone,
        Contexts = [InteractionContextType.Guild])]
    public Task MentionEveryoneAsync()
    {
        return RespondAsync(InteractionCallback.Message(new()
        {
            AllowedMentions = AllowedMentionsProperties.All,
            Content = "@everyone",
        }));
    }
}
