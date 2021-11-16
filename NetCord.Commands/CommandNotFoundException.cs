namespace NetCord.Commands
{
    internal class CommandNotFoundException : Exception
    {
        public CommandNotFoundException() : base("Command not found")
        {
        }
    }
}