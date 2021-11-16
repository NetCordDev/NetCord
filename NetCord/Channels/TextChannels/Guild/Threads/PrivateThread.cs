namespace NetCord
{
    public class PrivateThread : Thread
    {
        internal PrivateThread(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {

        }
    }
}
