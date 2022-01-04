namespace NetCord
{
    public class NewsThread : Thread
    {
        internal NewsThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
