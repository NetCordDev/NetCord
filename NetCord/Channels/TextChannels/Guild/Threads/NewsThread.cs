namespace NetCord
{
    public class NewsThread : Thread
    {
        internal NewsThread(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}
