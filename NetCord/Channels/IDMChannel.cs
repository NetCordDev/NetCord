namespace NetCord;

public partial interface IDMChannel : ITextChannel, IPinnableChannel
{
    IReadOnlyDictionary<ulong, User> Users { get; }
}
