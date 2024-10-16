using NetCord.Rest;

namespace NetCord;

public interface IInteraction : IEntity, ISpanFormattable, IJsonModel<JsonModels.JsonInteraction>
{
    public static IInteraction CreateFromJson(JsonModels.JsonInteraction jsonModel, Func<IInteraction, InteractionCallback, RestRequestProperties?, CancellationToken, Task> sendResponseAsync, RestClient client)
    {
        if (jsonModel.Type is InteractionType.Ping)
            return new PingInteraction(jsonModel, sendResponseAsync, client);

        return Interaction.CreateFromJson(jsonModel, null, sendResponseAsync, client);
    }

    public ulong ApplicationId { get; }

    public User User { get; }

    public string Token { get; }

    public Permissions AppPermissions { get; }

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public Task SendResponseAsync(InteractionCallback callback, RestRequestProperties? properties = null, CancellationToken cancellationToken = default);
}
