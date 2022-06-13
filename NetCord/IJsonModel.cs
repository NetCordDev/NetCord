namespace NetCord;

public interface IJsonModel<T>
{
    T JsonModel { get; }
}