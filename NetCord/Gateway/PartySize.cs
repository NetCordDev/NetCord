namespace NetCord.Gateway;

public class PartySize(long[] jsonModel)
{
    public long CurrentSize { get; } = jsonModel[0];

    public long MaxSize { get; } = jsonModel[1];
}
