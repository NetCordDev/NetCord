namespace NetCord
{
    public class NewsGuildChannel : TextGuildChannel
    {
        internal NewsGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
