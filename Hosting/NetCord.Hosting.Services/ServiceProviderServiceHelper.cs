using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Services;

internal static class ServiceProviderServiceHelper
{
    public static T GetSingle<T>(IServiceProvider services)
    {
        var enumerable = services.GetRequiredService<IEnumerable<T>>();
        T result;
        using var enumerator = enumerable.GetEnumerator();

        if (!enumerator.MoveNext())
            throw new InvalidOperationException($"No service of type '{typeof(T)}' was registered.");

        result = enumerator.Current;

        if (enumerator.MoveNext())
            throw new InvalidOperationException($"Multiple services of type '{typeof(T)}' were registered. Consider using a generic overload to specify the service to use.");

        return result;
    }
}
