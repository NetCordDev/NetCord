namespace NetCord.Interactions
{
    internal class InteractionNotFoundException : Exception
    {
        public InteractionNotFoundException() : base("Interaction not found")
        {
        }
    }
}