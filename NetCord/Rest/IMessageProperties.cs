namespace NetCord.Rest;

public interface IMessageProperties
{
    public bool Tts { get; set; }

    public string? Content { get; set; }

    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    public AllowedMentionsProperties? AllowedMentions { get; set; }

    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public MessageFlags? Flags { get; set; }
}
