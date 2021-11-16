namespace NetCord
{
    public class ImageAttachment : Attachment
    {
        public int Height => (int)_jsonEntity.Height;
        public int Width => (int)_jsonEntity.Width;

        internal ImageAttachment(JsonModels.JsonAttachment jsonEntity) : base(jsonEntity)
        {

        }
    }
}
