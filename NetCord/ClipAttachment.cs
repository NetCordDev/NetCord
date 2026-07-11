using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an <see cref="Attachment"/> with properties relevant to stream clips.
/// </summary>
public class ClipAttachment : Attachment
{
    public ClipAttachment(JsonModels.JsonAttachment jsonModel, RestClient client) : base(jsonModel)
    {
        ClipParticipants = jsonModel.ClipParticipants!.Select(p => new User(p, client)).ToArray();

        var application = jsonModel.Application;
        if (application is not null)
            Application = new(application, client);
    }

    /// <summary>
    /// A list of users present in the stream clip.
    /// </summary>
    public IReadOnlyList<User> ClipParticipants { get; }

    /// <summary>
    /// When the clip was created.
    /// </summary>
    public DateTimeOffset ClipCreatedAt => _jsonModel.ClipCreatedAt.GetValueOrDefault();

    /// <summary>
    /// The application in the stream clip, if recognized.
    /// </summary>
    public Application? Application { get; }
}
