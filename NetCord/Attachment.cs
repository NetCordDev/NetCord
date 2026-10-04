using System.Globalization;
using System.Web;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a message attachment, and its contained data.
/// </summary>
public class Attachment(JsonAttachment jsonModel) : Entity(jsonModel)
{
    /// <summary>
    /// Name of the attachment (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).
    /// </summary>
    public string FileName { get; } = jsonModel.FileName;

    /// <summary>
    /// The title of the attachment.
    /// </summary>
    public string? Title { get; } = jsonModel.Title;

    /// <summary>
    /// Description for the attachment (max 1024 characters for attachments sent by message, max 200 characters for attachments used for sticker creation).
    /// </summary>
    public string? Description { get; } = jsonModel.Description;

    /// <summary>
    /// The attachment's media (MIME) type.
    /// </summary>
    public string? ContentType { get; } = jsonModel.ContentType;

    /// <summary>
    /// The attachment's size in bytes.
    /// </summary>
    public long Size { get; } = jsonModel.Size;

    /// <summary>
    /// The attachment's source URL.
    /// </summary>
    public string Url { get; } = jsonModel.Url;

    /// <summary>
    /// The attachment's source URL, proxied through Discord's CDN.
    /// </summary>
    public string ProxyUrl { get; } = jsonModel.ProxyUrl;

    /// <summary>
    /// Whether this attachment is ephemeral.
    /// </summary>
    public bool? Ephemeral { get; } = jsonModel.Ephemeral;

    /// <summary>
    /// Additional information about the attachment's type.
    /// </summary>
    public AttachmentFlags? Flags { get; } = jsonModel.Flags;

    /// <summary>
    /// Returns expiration and issue info for the attachment's source URL.
    /// </summary>
    public AttachmentExpirationInfo GetExpirationInfo() => new(Url);

    public static Attachment Create(JsonAttachment jsonModel, RestClient client)
    {
        if (jsonModel.Width.HasValue)
            return new ImageAttachment(jsonModel);
        else if (jsonModel.DurationSeconds.HasValue)
            return new VoiceAttachment(jsonModel);
        else if (jsonModel.Flags is { } flags && flags.HasFlag(AttachmentFlags.Clip))
            return new ClipAttachment(jsonModel, client);
        else
            return new Attachment(jsonModel);
    }
}

/// <summary>
/// Contains information on a CDN URL's issue and expiry.
/// </summary>
public class AttachmentExpirationInfo
{
    public AttachmentExpirationInfo(string url)
    {
        var query = HttpUtility.ParseQueryString(new Uri(url).Query);

        var culture = CultureInfo.InvariantCulture;

        ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["ex"]!, NumberStyles.AllowHexSpecifier, culture));
        IssuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["is"]!, NumberStyles.AllowHexSpecifier, culture));
        Signature = query["hm"]!;
    }

    /// <summary>
    /// A timestamp indicating when the CDN URL will expire.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; }

    /// <summary>
    /// A timestamp indicating when the CDN URL was issued.
    /// </summary>
    public DateTimeOffset IssuedAt { get; }

    /// <summary>
    /// A unique signature, valid until the CDN URL's expiration.
    /// </summary>
    public string Signature { get; }
}

/// <summary>
/// Represents an <see cref="Attachment"/> with properties relevant to image/video files.
/// </summary>
public class ImageAttachment(JsonAttachment jsonModel) : Attachment(jsonModel)
{
    /// <summary>
    /// The height of the attachment in pixels.
    /// </summary>
    public int Height { get; } = jsonModel.Height.GetValueOrDefault();

    /// <summary>
    /// The width of the attachment in pixels.
    /// </summary>
    public int Width { get; } = jsonModel.Width.GetValueOrDefault();

    /// <summary>
    /// The attachment's <see href="https://evanw.github.io/thumbhash/">thumbhash</see> placeholder.
    /// </summary>
    public string? Placeholder { get; } = jsonModel.Placeholder;

    /// <summary>
    /// The <see cref="Placeholder"/>'s version.
    /// </summary>
    public int? PlaceholderVersion { get; } = jsonModel.PlaceholderVersion;
}

/// <summary>
/// Represents an attachment with properties relevant to voice messages.
/// </summary>
/// <param name="jsonModel"></param>
public class VoiceAttachment(JsonAttachment jsonModel) : Attachment(jsonModel)
{
    /// <summary>
    /// The duration of the audio file.
    /// </summary>
    public TimeSpan Duration { get; } = TimeSpan.FromSeconds(jsonModel.DurationSeconds.GetValueOrDefault());

    /// <summary>
    /// Byte array representing a sampled waveform. It is intended to be a preview of the entire voice message. Clients sample the recording at most once per 100 milliseconds, but will downsample so that no more than 256 datapoints are in the waveform.
    /// </summary>
    public IReadOnlyList<byte> Waveform { get; } = jsonModel.Waveform!;
}

/// <summary>
/// Represents an <see cref="Attachment"/> with properties relevant to stream clips.
/// </summary>
public class ClipAttachment(JsonAttachment jsonModel, RestClient client) : Attachment(jsonModel)
{
    /// <summary>
    /// A list of users present in the stream clip.
    /// </summary>
    public IReadOnlyList<User> ClipParticipants { get; } = [.. jsonModel.ClipParticipants!.Select(p => new User(p, client))];

    /// <summary>
    /// When the clip was created.
    /// </summary>
    public DateTimeOffset ClipCreatedAt { get; } = jsonModel.ClipCreatedAt.GetValueOrDefault();

    /// <summary>
    /// The application in the stream clip, if recognized.
    /// </summary>
    public Application? Application { get; } = jsonModel.Application is { } application ? new(application, client) : null;
}
