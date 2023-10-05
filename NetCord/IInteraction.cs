using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public interface IInteraction : IEntity, IJsonModel<JsonModels.JsonInteraction>
{
    public ulong ApplicationId { get; }

    public User User { get; }

    public string Token { get; }

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public static IInteraction CreateFromJson(JsonModels.JsonInteraction jsonModel, RestClient client)
    {
        if (jsonModel.Type is InteractionType.Ping)
            return new PingInteraction(jsonModel, client);

        return Interaction.CreateFromJson(jsonModel, (Guild?)null, client);
    }
}
