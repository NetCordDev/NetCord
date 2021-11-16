namespace NetCord.Interactions
{
    public class InteractionAttribute : Attribute
    {
        public string CustomId { get; }

        public InteractionAttribute(string customId)
        {
            CustomId = customId;
        }
    }
}