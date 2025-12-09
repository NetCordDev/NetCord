using NetCord;
using NetCord.Services.ApplicationCommands;

namespace Commands;

/// <summary>
/// Examples for the User Commands guide.
/// </summary>
public class UserCommands : ApplicationCommandModule<UserCommandContext>
{
    // TODO: Add user command examples (context menu on users)
    // - Basic user commands
    // - Accessing user information
    // - Responding to user commands
    
    [UserCommand("Get User Info")]
    public string GetUserInfo(User target)
    {
        return $"User: {target.Username} (ID: {target.Id})";
    }
}
