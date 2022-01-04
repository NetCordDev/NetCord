namespace NetCord
{
    public class StoreGuildChannel : TextGuildChannel
    {
        internal StoreGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
