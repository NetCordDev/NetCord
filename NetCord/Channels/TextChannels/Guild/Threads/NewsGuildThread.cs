namespace NetCord
{
    public class NewsGuildThread : Thread
    {
        internal NewsGuildThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
