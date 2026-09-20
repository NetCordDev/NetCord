using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an <see cref="Attachment"/> with properties relevant to stream clips.
/// </summary>
public class ClipAttachment(JsonAttachment jsonModel, RestClient client) : Attachment(jsonModel)
{
    /// <summary>
    /// A list of users present in the stream clip.
    /// </summary>
    public IReadOnlyList<User> ClipParticipants { get; } = jsonModel.ClipParticipants!.Select(p => new User(p, client)).ToArray();

    /// <summary>
    /// When the clip was created.
    /// </summary>
    public DateTimeOffset ClipCreatedAt => jsonModel.ClipCreatedAt.GetValueOrDefault();

    /// <summary>
    /// The application in the stream clip, if recognized.
    /// </summary>
    public Application? Application { get; } = jsonModel.Application is { } application ? new(application, client) : null;
}
