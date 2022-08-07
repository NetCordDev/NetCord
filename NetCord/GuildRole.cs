using System.Runtime.CompilerServices;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildRole : ClientEntity, IJsonModel<JsonGuildRole>
{
    JsonGuildRole IJsonModel<JsonGuildRole>.JsonModel => _jsonModel;
    private readonly JsonGuildRole _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public Color Color => _jsonModel.Color;

    public bool Hoist => _jsonModel.Hoist;

    public int Position => _jsonModel.Position;

    public Permission Permissions => _jsonModel.Permissions;

    public bool Managed => _jsonModel.Managed;

    public bool Mentionable => _jsonModel.Mentionable;

    public GuildRoleTags? Tags { get; }

    public Snowflake GuildId { get; }

    public GuildRole(JsonGuildRole jsonModel, Snowflake guildId, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Tags != null)
            Tags = new(jsonModel.Tags);
        GuildId = guildId;
    }

    public override string ToString() => $"<@&{Id}>";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(GuildRole left, GuildRole right) => left.Position > right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(GuildRole left, GuildRole right) => left.Position < right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(GuildRole left, GuildRole right) => left.Position >= right.Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(GuildRole left, GuildRole right) => left.Position <= right.Position;

    #region Guild
    public Task<GuildRole> ModifyAsync(Action<GuildRoleOptions> action, RequestProperties? properties = null) => _client.ModifyGuildRoleAsync(GuildId, Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildRoleAsync(GuildId, Id, properties);
    #endregion
}

public class GuildRoleTags : IJsonModel<JsonTags>
{
    JsonTags IJsonModel<JsonTags>.JsonModel => _jsonModel;
    private readonly JsonTags _jsonModel;

    public Snowflake? BotId => _jsonModel.BotId;

    public Snowflake? IntegrationId => _jsonModel.IntegrationId;

    public bool IsPremiumSubscriber => _jsonModel.IsPremiumSubscriber;

    public GuildRoleTags(JsonTags jsonModel)
    {
        _jsonModel = jsonModel;
    }
}