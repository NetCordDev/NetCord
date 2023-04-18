# Permissions

Instead of using Precondition Attributes, you can specify command required permissions to Discord. The commands will not show up to users without the permissions then. You can also specify if commands can be used in DM. Example:
```cs
[SlashCommand("mention-everyone", "Mentions @everyone",
    DefaultGuildUserPermissions = Permission.MentionEveryone,
    DMPermission = false)]
public Task MentionEveryoneAsync()
{
    return RespondAsync(InteractionCallback.ChannelMessageWithSource(new()
    {
        AllowedMentions = AllowedMentionsProperties.All,
        Content = "@everyone",
    }));
}
```