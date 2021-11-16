namespace NetCord
{
    public interface IMessageComponent
    {
        public MessageComponentType ComponentType { get; }

        internal static IMessageComponent CreateFromJson(JsonModels.JsonMessageComponent jsonEntity)
        {
            if (jsonEntity.Components[0].Type == MessageComponentType.Menu)
            {
                return new MessageMenu(jsonEntity);
            }
            else
            {
                return new MessageActionRow(jsonEntity);
            }
        }
    }
}
