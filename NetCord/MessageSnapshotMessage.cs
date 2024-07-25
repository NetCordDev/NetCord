using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageSnapshotMessage(JsonMessageSnapshotMessage jsonModel, ulong? guildId, RestClient client) : IJsonModel<JsonMessageSnapshotMessage>
{
    JsonMessageSnapshotMessage IJsonModel<JsonMessageSnapshotMessage>.JsonModel => jsonModel;

    /// <summary>
    /// The type of the message.
    /// </summary>
    public MessageType Type => jsonModel.Type;

    /// <summary>
    /// The text contents of the message.
    /// </summary>
    public string Content => jsonModel.Content;

    /// <summary>
    /// A list of <see cref="Embed"/> objects containing any embedded content present in the message.
    /// </summary>
    public IReadOnlyList<Embed> Embeds { get; } = jsonModel.Embeds.Select(e => new Embed(e)).ToArray();

    /// <summary>
    /// A dictionary of <see cref="Attachment"/> objects indexed by their IDs, containing any files attached in the message.
    /// </summary>
    public IReadOnlyDictionary<ulong, Attachment> Attachments { get; } = jsonModel.Attachments.ToDictionary(a => a.Id, Attachment.CreateFromJson);

    /// <summary>
    /// When the message was edited (or null if never).
    /// </summary>
    public DateTimeOffset? EditedAt => jsonModel.EditedAt;

    /// <summary>
    /// A <see cref="MessageFlags"/> object indicating the message's applied flags.
    /// </summary>
    public MessageFlags Flags => jsonModel.Flags.GetValueOrDefault();

    /// <summary>
    /// A dictionary of <see cref="User"/> objects indexed by their IDs, containing users specifically mentioned in the message.
    /// </summary>
    public IReadOnlyDictionary<ulong, User> MentionedUsers { get; } = jsonModel.MentionedUsers.ToDictionary(u => u.Id, u =>
    {
        var guildUser = u.GuildUser;
        if (guildUser is null)
            return new User(u, client);

        guildUser.User = u;
        return new GuildUser(guildUser, guildId.GetValueOrDefault(), client);
    });

    /// <summary>
    /// A list of IDs corresponding to roles specifically mentioned in the message.
    /// </summary>
    public IReadOnlyList<ulong> MentionedRoleIds => jsonModel.MentionedRoleIds;
}
