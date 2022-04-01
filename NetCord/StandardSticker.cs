namespace NetCord
{
    public class StandardSticker : Sticker
    {
        public Snowflake PackId => _jsonEntity.PackId.GetValueOrDefault();
        public int? SortValue => _jsonEntity.SortValue;

        internal StandardSticker(JsonModels.JsonSticker jsonEntity) : base(jsonEntity)
        {

        }
    }
}