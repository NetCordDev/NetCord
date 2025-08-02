namespace ServicesTest;

internal static class Body
{
    public static void Data<TData>(TData data)
    {
        throw new ResultDataException<TData>(data);
    }
}

internal class ResultDataException<TData>(TData data) : Exception
{
    public new TData Data => data;
}
