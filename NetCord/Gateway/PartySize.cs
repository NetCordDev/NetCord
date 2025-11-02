namespace NetCord.Gateway;

public class PartySize(long[] jsonModel) : IJsonModel<long[]>
{
    long[] IJsonModel<long[]>.JsonModel => jsonModel;

    public long CurrentSize => jsonModel[0];

    public long MaxSize => jsonModel[1];
}
