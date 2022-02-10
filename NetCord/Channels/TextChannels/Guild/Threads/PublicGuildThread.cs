namespace NetCord
{
    public class PublicGuildThread : Thread
    {
        internal PublicGuildThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
