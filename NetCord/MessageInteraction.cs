namespace NetCord
{
    public class MessageInteraction : Entity
    {
        private readonly JsonModels.JsonMessageInteraction _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;
        public InteractionType Type => _jsonEntity.Type;
        public string Name => _jsonEntity.Name;
        public User User { get; }

        internal MessageInteraction(JsonModels.JsonMessageInteraction jsonEntity, RestClient client)
        {
            _jsonEntity = jsonEntity;
            User = new(jsonEntity.User, client);
        }
    }
}