using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCord.Natives;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class NativeLibraryVersionAttribute(string name, string version) : Attribute
{
    public string Name { get; } = name;
    public string Version { get; } = version;
}

public static class NativesHelper
{
    public static IEnumerable<NativeLibraryVersionAttribute> GetNativeLibraryVersions()
    {
        return typeof(NativesHelper).Assembly.GetCustomAttributes<NativeLibraryVersionAttribute>();
    }
}
