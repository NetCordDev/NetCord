namespace MyBot;

public interface IDataProvider
{
    public IReadOnlyList<string> GetData();
}

public class DataProvider : IDataProvider
{
    public IReadOnlyList<string> GetData() => ["hello", "world"];
}
