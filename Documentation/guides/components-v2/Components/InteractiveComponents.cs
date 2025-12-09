using NetCord;
using NetCord.Rest;

namespace Components;

/// <summary>
/// Examples for the Interactive Components guide.
/// </summary>
public static class InteractiveComponents
{
    // TODO: Add interactive component examples
    // - Buttons
    // - Select menus (string, user, role, channel, mentionable)
    // - Custom IDs and payloads
    // - Component state management
    
    public static MessageProperties CreateButtonExample()
    {
        return new MessageProperties
        {
            Content = "Click a button:",
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("btn_primary", "Primary", ButtonStyle.Primary),
                        new ButtonProperties("btn_secondary", "Secondary", ButtonStyle.Secondary),
                        new ButtonProperties("btn_success", "Success", ButtonStyle.Success),
                        new ButtonProperties("btn_danger", "Danger", ButtonStyle.Danger)
                    ]
                }
            ]
        };
    }
    
    public static MessageProperties CreateSelectMenuExample()
    {
        return new MessageProperties
        {
            Content = "Choose an option:",
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new StringMenuProperties("menu_select", 
                        [
                            new("Option 1", "opt1", "First option"),
                            new("Option 2", "opt2", "Second option"),
                            new("Option 3", "opt3", "Third option")
                        ])
                        {
                            Placeholder = "Select an option..."
                        }
                    ]
                }
            ]
        };
    }
}
