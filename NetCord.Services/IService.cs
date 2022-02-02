using System.Reflection;

namespace NetCord.Services;

public interface IService
{
    public void AddModules(Assembly assembly);

    public void AddModule(Type type);
}