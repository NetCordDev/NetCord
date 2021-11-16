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
            Footer = new(jsonEntity.Footer);
            Image = new(jsonEntity.Image);
            Thumbnail = new(jsonEntity.Thumbnail);
            Video = new(jsonEntity.Video);
            Provider = new(jsonEntity.Provider);
            Author = new(jsonEntity.Author);
            Fields = jsonEntity.Fields.SelectOrEmpty(f => new MessageEmbedField(f));
        }
    }
}