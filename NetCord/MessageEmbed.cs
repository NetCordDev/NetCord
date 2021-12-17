namespace NetCord
{
    public class MessageEmbed
    {
        private readonly JsonModels.JsonEmbed _jsonEntity;

        public string? Title => _jsonEntity.Title;
        public MessageEmbedType? Type => _jsonEntity.Type;
        public string? Description => _jsonEntity.Description;
        public string? Url => _jsonEntity.Url;
        public DateTimeOffset? Timestamp => _jsonEntity.Timestamp;
        public Color? Color => _jsonEntity.Color;
        public MessageEmbedFooter? Footer { get; }
        public MessageEmbedImage? Image { get; }
        public MessageEmbedThumbnail? Thumbnail { get; }
        public MessageEmbedVideo? Video { get; }
        public MessageEmbedProvider? Provider { get; }
        public MessageEmbedAuthor? Author { get; }
        public IEnumerable<MessageEmbedField> Fields { get; }

        internal MessageEmbed(JsonModels.JsonEmbed jsonEntity)
        {
            _jsonEntity = jsonEntity;
            if (jsonEntity.Footer != null) Footer = new(jsonEntity.Footer);
            if (jsonEntity.Image != null) Image = new(jsonEntity.Image);
            if (jsonEntity.Thumbnail != null) Thumbnail = new(jsonEntity.Thumbnail);
            if (jsonEntity.Video != null) Video = new(jsonEntity.Video);
            if (jsonEntity.Provider != null) Provider = new(jsonEntity.Provider);
            if (jsonEntity.Author != null) Author = new(jsonEntity.Author);
            Fields = jsonEntity.Fields.Select(f => new MessageEmbedField(f));
        }
    }
}