using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
