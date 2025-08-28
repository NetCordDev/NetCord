using NetCord;
using NetCord.Rest;

#pragma warning disable IDE0017, IDE0040, IDE0051, IDE0059, CS8321

static async Task RespondAsync()
{
    Interaction interaction = null!;
    InteractionCallbackProperties callback = null!;

    await interaction.SendResponseAsync(callback);
}

static void Callbacks()
{
    InteractionCallbackProperties callback;

    callback = InteractionCallback.Message("Here is a sample message interaction callback!");

    callback = InteractionCallback.DeferredMessage();

    callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);

    callback = InteractionCallback.DeferredModifyMessage;

    callback = InteractionCallback.ModifyMessage(message => message.Content = "New content!");

    callback = InteractionCallback.ModifyMessage(message => message.WithContent("New content!"));

    callback = InteractionCallback.Modal(new("intro", "Introduce Yourself")
    {
        new LabelProperties("First Name", new TextInputProperties("name", TextInputStyle.Short)),
        new LabelProperties("Your Bio", new TextInputProperties("bio", TextInputStyle.Paragraph)),
    });

    callback = InteractionCallback.Modal(new ModalProperties("intro", "Introduce Yourself")
        .AddComponents(
            new LabelProperties("First Name", new TextInputProperties("name", TextInputStyle.Short)),
            new LabelProperties("Your Bio", new TextInputProperties("bio", TextInputStyle.Paragraph))));

    callback = InteractionCallback.Modal(new("intro", "Introduce Yourself")
    {
        new LabelProperties("First Name", new TextInputProperties("name", TextInputStyle.Short)
        {
            MinLength = 2,
            MaxLength = 32,
            Placeholder = "Enter your name",
        }),
        new LabelProperties("Your Bio", new TextInputProperties("bio", TextInputStyle.Paragraph)
        {
            MinLength = 10,
            Required = false,
            Value = "I love programming!",
        }),
    });

    callback = InteractionCallback.Modal(new ModalProperties("intro", "Introduce Yourself")
        .AddComponents(
            new LabelProperties("First Name", new TextInputProperties("name", TextInputStyle.Short)
                .WithMinLength(2)
                .WithMaxLength(32)
                .WithPlaceholder("Enter your name")),
            new LabelProperties("Your Bio", new TextInputProperties("bio", TextInputStyle.Paragraph)
                .WithMinLength(10)
                .WithRequired(false)
                .WithValue("I love programming!"))));

    callback = InteractionCallback.Autocomplete([new("Dog", "dog"), new("Cat", "cat")]);

    callback = InteractionCallback.Autocomplete([new("Frog", 0), new("Duck", 1)]);

    callback = InteractionCallback.Autocomplete(
    [
        new("Lion", "lion")
        {
            NameLocalizations = new Dictionary<string, string> { ["pl"] = "Lew" },
        },
        new("Elephant", "elephant")
        {
            NameLocalizations = new Dictionary<string, string> { ["pl"] = "Słoń" },
        },
    ]);

    callback = InteractionCallback.Autocomplete(
    [
        new ApplicationCommandOptionChoiceProperties("Lion", "lion")
            .WithNameLocalizations(new Dictionary<string, string> { ["pl"] = "Lew" }),
        new ApplicationCommandOptionChoiceProperties("Elephant", "elephant")
            .WithNameLocalizations(new Dictionary<string, string> { ["pl"] = "Słoń" }),
    ]);

    callback = InteractionCallback.Pong;
}

static async Task RespondDeferredAsync()
{
    Interaction interaction = null!;

    await interaction.ModifyResponseAsync(message => message.Content = "The response was modified!");

    await interaction.ModifyResponseAsync(message => message.WithContent("The response was modified!"));

    await interaction.SendFollowupMessageAsync("The response was provided via follow-up!");
}
