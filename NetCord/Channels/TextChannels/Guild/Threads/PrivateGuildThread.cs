namespace NetCord
{
    public class PrivateGuildThread : Thread
    {
        internal PrivateGuildThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
