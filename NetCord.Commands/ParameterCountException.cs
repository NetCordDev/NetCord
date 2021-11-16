namespace NetCord.Commands
{
    internal class ParameterCountException : Exception
    {
        internal ParameterCountException(string message) : base(message)
        {
        }
    }
}
