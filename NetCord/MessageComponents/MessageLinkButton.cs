namespace NetCord
{
    public class MessageLinkButton : MessageButton
    {
        public int Style => 5;
        public string Url => _jsonEntity.Url;

        internal MessageLinkButton(JsonModels.JsonMessageComponent jsonEntity) : base(jsonEntity)
        {

        }
    }
}
