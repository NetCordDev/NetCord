using NetCord;
using NetCord.Rest;

namespace Components;

/// <summary>
/// Examples for the Components Overview guide.
/// </summary>
public static class Overview
{
    // TODO: Add component system overview
    // - What are components
    // - Component types
    // - Component architecture
    // - Best practices
    
    public static MessageProperties CreateBasicComponentMessage()
    {
        return new MessageProperties
        {
            Content = "Choose an action:",
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("action_yes", "Yes", ButtonStyle.Success),
                        new ButtonProperties("action_no", "No", ButtonStyle.Danger)
                    ]
                }
            ]
        };
    }
}
