using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents properties used to create a channel invite.
/// </summary>
[GenerateMethodsForProperties]
public partial class InviteProperties : IHttpSerializable
{
    /// <summary>
    /// The duration, in seconds, after which the invite expires.
    /// </summary>
    /// <remarks>
    /// Must be between <c>0</c> and <c>604800</c> seconds. A value of <c>0</c> means the invite never expires.
    /// Discord defaults this value to <c>86400</c> seconds.
    /// </remarks>
    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    /// <summary>
    /// The maximum number of times the invite can be used.
    /// </summary>
    /// <remarks>
    /// Must be between <c>0</c> and <c>100</c>. A value of <c>0</c> allows unlimited uses.
    /// </remarks>
    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    /// <summary>
    /// Whether the invite grants temporary membership.
    /// </summary>
    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    /// <summary>
    /// Whether Discord should always create a unique invite instead of reusing a similar existing invite.
    /// </summary>
    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }

    /// <summary>
    /// The target type of the voice channel invite.
    /// </summary>
    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    /// <summary>
    /// The ID of the user whose stream should be displayed for a stream invite.
    /// </summary>
    /// <remarks>
    /// Required when <see cref="TargetType"/> is a stream invite. The user must be streaming in the channel.
    /// </remarks>
    [JsonPropertyName("target_user_id")]
    public ulong? TargetUserId { get; set; }

    /// <summary>
    /// The ID of the embedded application to open for an embedded application invite.
    /// </summary>
    /// <remarks>
    /// Required when <see cref="TargetType"/> is an embedded application invite. The application must have the <c>EMBEDDED</c> flag.
    /// </remarks>
    [JsonPropertyName("target_application_id")]
    public ulong? TargetApplicationId { get; set; }

    /// <summary>
    /// The users allowed to see and accept the invite.
    /// </summary>
    /// <remarks>
    /// These users are serialized as the <c>target_users_file</c> CSV part of a multipart request.
    /// Duplicate user IDs are ignored by Discord.
    /// </remarks>
    [JsonIgnore]
    public InviteTargetUsersProperties? TargetUsers { get; set; }

    /// <summary>
    /// The IDs of roles granted to users who accept the invite.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MANAGE_ROLES</c> permission. Roles with higher permissions than the sender cannot be assigned.
    /// </remarks>
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
