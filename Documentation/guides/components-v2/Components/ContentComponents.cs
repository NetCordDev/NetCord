using NetCord;
using NetCord.Rest;

namespace Components;

/// <summary>
/// Examples for the Content Components guide.
/// </summary>
public static class ContentComponents
{
    // TODO: Add content component examples
    // - Text content
    // - Embeds with components
    // - Images with components
    // - Combining content types
    
    public static MessageProperties CreateContentWithComponents()
    {
        return new MessageProperties
        {
            Content = "**Rich content example**",
            Embeds =
            [
                new EmbedProperties
                {
                    Title = "Information",
                    Description = "This embed has components below it."
                }
            ],
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("more_info", "More Info", ButtonStyle.Link)
                        {
                            Url = "https://example.com"
                        }
                    ]
                }
            ]
        };
    }
}
