using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageSnapshotMessage(JsonMessageSnapshotMessage jsonModel, ulong? guildId, RestClient client)
{
    /// <summary>
    /// The type of the message.
    /// </summary>
    public MessageType Type { get; } = jsonModel.Type;

    /// <summary>
    /// The text contents of the message.
    /// </summary>
    public string Content { get; } = jsonModel.Content;

    /// <summary>
    /// A list of <see cref="Embed"/> objects containing any embedded content present in the message.
    /// </summary>
    public IReadOnlyList<Embed> Embeds { get; } = [.. jsonModel.Embeds.Select(e => new Embed(e))];

    /// <summary>
    /// A list of <see cref="Attachment"/> objects indexed by their IDs, containing any files attached in the message.
    /// </summary>
    public IReadOnlyList<Attachment> Attachments { get; } = [.. jsonModel.Attachments.Select(a => Attachment.CreateFromJson(a, client))];

    /// <summary>
    /// When the message was edited (or null if never).
    /// </summary>
    public DateTimeOffset? EditedAt { get; } = jsonModel.EditedAt;

    /// <summary>
    /// A <see cref="MessageFlags"/> object indicating the message's applied flags.
    /// </summary>
    public MessageFlags? Flags { get; } = jsonModel.Flags;

    /// <summary>
    /// A list of <see cref="User"/> objects indexed by their IDs, containing users specifically mentioned in the message.
    /// </summary>
    public IReadOnlyList<User> MentionedUsers { get; } = [.. jsonModel.MentionedUsers.Select(u =>
    {
        if (u.GuildUser is { } guildUser)
        {
            guildUser.User = u;
            return new GuildUser(guildUser, guildId.GetValueOrDefault(), client);
        }

        return new User(u, client);
    })];

    /// <summary>
    /// A list of IDs corresponding to roles specifically mentioned in the message.
    /// </summary>
    public IReadOnlyList<ulong> MentionedRoleIds { get; } = jsonModel.MentionedRoleIds;

    /// <summary>
    /// Contains stickers contained in the message, if any.
    /// </summary>
    public IReadOnlyList<MessageSticker> Stickers { get; } = jsonModel.Stickers is { } stickers ? stickers.Select(s => new MessageSticker(s, client)).ToArray() : [];

    /// <summary>
    /// A list of <see cref="IMessageChildComponent"/> objects, contains components like <see cref="ButtonComponent"/>s, <see cref="ActionRowComponent"/>s, or other interactive components if any are present.
    /// </summary>
    public IReadOnlyList<IMessageChildComponent> Components { get; } = jsonModel.Components is { } components ? components.Select(IMessageChildComponent.CreateFromJson).ToArray() : [];
}
