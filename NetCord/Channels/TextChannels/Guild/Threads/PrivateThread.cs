namespace NetCord
{
    public class PrivateThread : Thread
    {
        internal PrivateThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
