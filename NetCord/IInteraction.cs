global using InteractionResponseDelegate = System.Func<NetCord.IInteraction, NetCord.Rest.InteractionCallbackProperties, bool, NetCord.Rest.RestRequestProperties?, System.Threading.CancellationToken, System.Threading.Tasks.Task<NetCord.Rest.InteractionCallbackResponse?>>;

using NetCord.Rest;

namespace NetCord;

public interface IInteraction : IEntity, ISpanFormattable, IJsonModel<JsonModels.JsonInteraction>
{
    public ulong ApplicationId { get; }

    public User User { get; }

    public string Token { get; }

    public int Version { get; }

    public Permissions AppPermissions { get; }

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public long AttachmentSizeLimit { get; }

    public static IInteraction CreateFromJson(JsonModels.JsonInteraction jsonModel, InteractionResponseDelegate sendResponseAsync, RestClient client)
    {
        if (jsonModel.Type is InteractionType.Ping)
            return new PingInteraction(jsonModel, sendResponseAsync, client);

        return Interaction.CreateFromJson(jsonModel, null, sendResponseAsync, client);
    }

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default);
}
