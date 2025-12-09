using NetCord;
using NetCord.Rest;

namespace Components;

/// <summary>
/// Examples for the Modal Components guide.
/// </summary>
public static class ModalComponents
{
    // TODO: Add modal component examples
    // - Creating modals
    // - Text inputs (short and paragraph)
    // - Modal submission handling
    // - Validation
    
    public static ModalProperties CreateBasicModal()
    {
        return new ModalProperties("modal_feedback", "Feedback Form")
        {
            Components =
            [
                new TextInputProperties("input_name", TextInputStyle.Short, "Name")
                {
                    MinLength = 2,
                    MaxLength = 50,
                    Placeholder = "Enter your name..."
                },
                new TextInputProperties("input_feedback", TextInputStyle.Paragraph, "Feedback")
                {
                    MinLength = 10,
                    MaxLength = 500,
                    Placeholder = "Tell us what you think..."
                }
            ]
        };
    }
}
