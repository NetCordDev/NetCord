namespace NetCord
{
    public class ButtonInteractionData
    {
        private protected readonly JsonModels.JsonInteractionData _jsonEntity;

        public string CustomId => _jsonEntity.CustomId!;

        internal ButtonInteractionData(JsonModels.JsonInteractionData jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}