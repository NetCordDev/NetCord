namespace MyBot;

public interface ISomeService
{
    public IReadOnlyList<string> GetSomeData();
}

public class SomeService : ISomeService
{
    public IReadOnlyList<string> GetSomeData() => ["hello", "world"];
}
