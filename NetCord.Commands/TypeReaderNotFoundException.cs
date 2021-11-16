namespace NetCord.Commands
{
    internal class TypeReaderNotFoundException : Exception
    {
        internal TypeReaderNotFoundException(string typeName) : base(typeName)
        {

        }
    }
}
