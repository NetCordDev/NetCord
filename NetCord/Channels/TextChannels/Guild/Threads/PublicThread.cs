namespace NetCord
{
    public class PublicThread : Thread
    {
        internal PublicThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
