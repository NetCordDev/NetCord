using NetCord.Interactions;

namespace NetCord.Test;

public class CustomMenuInteractionContext : MenuInteractionContext
{
    public SelfUser SelfUser => Client.User;

    public CustomMenuInteractionContext(MenuInteraction interaction, BotClient client) : base(interaction, client)
    {
    }
}

public class CustomButtonInteractionContext : ButtonInteractionContext
{
    public SelfUser SelfUser => Client.User;

    public CustomButtonInteractionContext(ButtonInteraction interaction, BotClient client) : base(interaction, client)
    {
    }
}