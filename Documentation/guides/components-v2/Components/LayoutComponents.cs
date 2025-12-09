using NetCord;
using NetCord.Rest;

namespace Components;

/// <summary>
/// Examples for the Layout Components guide.
/// </summary>
public static class LayoutComponents
{
    // TODO: Add layout component examples
    // - ActionRow
    // - Component spacing
    // - Component limits
    // - Layout patterns
    
    public static MessageProperties CreateMultiRowLayout()
    {
        return new MessageProperties
        {
            Content = "Layout example:",
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("row1_btn1", "Button 1", ButtonStyle.Primary),
                        new ButtonProperties("row1_btn2", "Button 2", ButtonStyle.Primary)
                    ]
                },
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("row2_btn1", "Button 3", ButtonStyle.Secondary)
                    ]
                }
            ]
        };
    }
}
