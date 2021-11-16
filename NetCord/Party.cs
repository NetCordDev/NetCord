namespace NetCord
{
    public class Party : Entity
    {
        private readonly JsonModels.JsonParty _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;
        public PartySize? Size;

        internal Party(JsonModels.JsonParty jsonEntity)
        {
            _jsonEntity = jsonEntity;
            Size = new(jsonEntity.Size);
        }
    }

    public class PartySize
    {
        private readonly JsonModels.JsonPartySize _jsonEntity;

        public int CurrentSize => _jsonEntity.CurrentSize;

        public int MaxSize => _jsonEntity.MaxSize;

        internal PartySize(JsonModels.JsonPartySize jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}