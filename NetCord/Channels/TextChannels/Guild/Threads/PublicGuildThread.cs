namespace NetCord
{
    public class PublicGuildThread : GuildThread
    {
        internal PublicGuildThread(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {

        }
    }
}
