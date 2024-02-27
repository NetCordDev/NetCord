﻿using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class ModalSubmitInteractions : ComponentInteractionModule<ModalSubmitInteractionContext>
{
    [ComponentInteraction("wzium")]
    public Task WziumAsync(UserId user)
    {
        return RespondAsync(InteractionCallback.Message($"{user} got wziummed with reason: {Context.Components[0].Value}"));
    }
}
