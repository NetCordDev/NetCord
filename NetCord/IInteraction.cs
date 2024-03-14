using NetCord.Rest;

namespace NetCord;

public interface IInteraction : IEntity, ISpanFormattable, IJsonModel<JsonModels.JsonInteraction>
{
    public ulong ApplicationId { get; }

    public User User { get; }

    public string Token { get; }

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public static IInteraction CreateFromJson(JsonModels.JsonInteraction jsonModel, Func<IInteraction, InteractionCallback, RestRequestProperties?, Task> sendResponseAsync, RestClient client)
    {
        if (jsonModel.Type is InteractionType.Ping)
            return new PingInteraction(jsonModel, sendResponseAsync, client);

        return Interaction.CreateFromJson(jsonModel, null, sendResponseAsync, client);
    }

    public Task SendResponseAsync(InteractionCallback callback, RestRequestProperties? properties = null);
}
