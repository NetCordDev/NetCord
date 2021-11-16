namespace NetCord
{
    public class PublicThread : Thread
    {
        internal PublicThread(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}
