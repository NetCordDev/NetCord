using NetCord;
using NetCord.Rest;

_ = RespondAsync();

Callbacks();

_ = RespondDeferredAsync();

static async Task RespondAsync()
{
    Interaction interaction = null!;
    InteractionCallback callback = null!;

    await interaction.SendResponseAsync(callback);
}

static void Callbacks()
{
    InteractionCallback callback;

    callback = InteractionCallback.Message("Here is a sample message interaction callback!");

    callback = InteractionCallback.DeferredMessage();

    callback = InteractionCallback.DeferredMessage(MessageFlags.Ephemeral);

    callback = InteractionCallback.DeferredModifyMessage;

    callback = InteractionCallback.ModifyMessage(message => message.Content = "New content!");

    callback = InteractionCallback.Modal(new ModalProperties("intro", "Introduce Yourself",
    [
        new("name", TextInputStyle.Short, "First Name"),
        new("bio", TextInputStyle.Paragraph, "Your Bio"),
    ]));

    callback = InteractionCallback.Autocomplete([new("Dog", "dog"), new("Cat", "cat")]);

    callback = InteractionCallback.Autocomplete([new("Frog", 0), new("Duck", 1)]);

    callback = InteractionCallback.Pong;
}

static async Task RespondDeferredAsync()
{
    Interaction interaction = null!;

    await interaction.ModifyResponseAsync(message => message.Content = "The response was modified!");

    await interaction.SendFollowupMessageAsync("The response was provided via follow-up!");
}
