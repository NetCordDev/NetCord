using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildRole : ClientEntity, IJsonModel<JsonGuildRole>
{
    JsonGuildRole IJsonModel<JsonGuildRole>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildRole _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public Color Color => _jsonModel.Color;

    public bool Hoist => _jsonModel.Hoist;

    public int Position => _jsonModel.Position;

    public Permission Permissions { get; }

    public bool Managed => _jsonModel.Managed;

    public bool Mentionable => _jsonModel.Mentionable;

    public GuildRoleTags? Tags { get; }

    public GuildRole(JsonGuildRole jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Tags != null)
            Tags = new(jsonModel.Tags);
        Permissions = (Permission)ulong.Parse(jsonModel.Permissions);
    }

    public override string ToString() => $"<@&{Id}>";

    public static bool operator >(GuildRole left, GuildRole right) => left.Position > right.Position;

    public static bool operator <(GuildRole left, GuildRole right) => left.Position < right.Position;

    public static bool operator >=(GuildRole left, GuildRole right) => left.Position >= right.Position;

    public static bool operator <=(GuildRole left, GuildRole right) => left.Position <= right.Position;
}

public class GuildRoleTags : IJsonModel<JsonModels.JsonTags>
{
    JsonModels.JsonTags IJsonModel<JsonModels.JsonTags>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonTags _jsonModel;

    public Snowflake? BotId => _jsonModel.BotId;

    public Snowflake? IntegrationId => _jsonModel.IntegrationId;

    public bool IsPremiumSubscriber => _jsonModel.IsPremiumSubscriber;

    public GuildRoleTags(JsonModels.JsonTags jsonModel)
    {
        _jsonModel = jsonModel;
    }
}