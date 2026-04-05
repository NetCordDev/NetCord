using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class InviteProperties : IHttpSerializable
{
    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user_id")]
    public ulong? TargetUserId { get; set; }

    [JsonPropertyName("target_application_id")]
    public ulong? TargetApplicationId { get; set; }

    [JsonIgnore]
    public InviteTargetUsersProperties? TargetUsers { get; set; }

    [JsonPropertyName("role_ids")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    HttpContent IHttpSerializable.Serialize() => Serialize();

    internal HttpContent Serialize()
    {
        JsonContent<InviteProperties> inviteContent = new(this, Serialization.Default.InviteProperties);

        if (TargetUsers is not { } targetUsers)
            return inviteContent;

        return new MultipartFormDataContent()
        {
            { inviteContent, "payload_json" },
            { targetUsers.Serialize(), "target_users_file", "target_users_file" }
        };
    }
}
