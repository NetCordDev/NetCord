namespace NetCord.Rest;

public partial interface IMessageProperties
{
    public string? Content { get; set; }

    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    public AllowedMentionsProperties? AllowedMentions { get; set; }

    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public IEnumerable<ComponentProperties>? Components { get; set; }

    public MessageFlags? Flags { get; set; }
}
