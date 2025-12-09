using NetCord;
using NetCord.Rest;

namespace ComponentsBasics;

public static class ComponentExamples
{
    // TODO: Add comprehensive component examples
    public static MessageProperties CreateButtonExample()
    {
        return new MessageProperties
        {
            Content = "Click the button!",
            Components =
            [
                new ActionRowProperties
                {
                    Components =
                    [
                        new ButtonProperties("custom_id", "Click Me", ButtonStyle.Primary)
                    ]
                }
            ]
        };
    }
}
