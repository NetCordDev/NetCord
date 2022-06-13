namespace NetCord
{
    public class ImageAttachment : Attachment
    {
        public int Height => (int)_jsonModel.Height!;
        public int Width => (int)_jsonModel.Width!;

        public ImageAttachment(JsonModels.JsonAttachment jsonModel) : base(jsonModel)
        {

        }
    }
}
