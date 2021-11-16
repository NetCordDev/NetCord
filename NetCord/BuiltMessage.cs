namespace NetCord
{
    public class BuiltMessage
    {
        internal MultipartFormDataContent _content;

        internal BuiltMessage(MultipartFormDataContent content)
        {
            _content = content;
        }
    }
}
