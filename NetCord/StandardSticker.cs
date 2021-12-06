namespace NetCord
{
    public class StandardSticker : Sticker
    {
        public DiscordId PackId => _jsonEntity.PackId;
        public int? SortValue => _jsonEntity.SortValue;

        internal StandardSticker(JsonModels.JsonSticker jsonEntity) : base(jsonEntity)
        {

        }
    }
}