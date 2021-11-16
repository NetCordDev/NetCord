namespace NetCord
{
    public class StoreGuildChannel : TextGuildChannel
    {
        internal StoreGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}
