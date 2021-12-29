namespace NetCord.Test.Commands
{
    [Serializable]
    internal class RequiredContextException : Exception
    {
        public RequiredContextException(RequiredContext context) : base("Required context: " + context.ToString())
        {
        }
    }

    public enum RequiredContext
    {
        Guild,
        DM,
        GroupDM,
        Nsfw,
    }
}