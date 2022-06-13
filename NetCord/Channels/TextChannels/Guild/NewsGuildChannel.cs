namespace NetCord
{
    public class NewsGuildChannel : TextGuildChannel
    {
        public NewsGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
        {

        }
    }
}
