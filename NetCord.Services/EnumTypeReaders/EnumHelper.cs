using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.EnumTypeReaders;

internal static class EnumHelper
{
    [UnconditionalSuppressMessage("Trimming", "IL2070:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "Literal fields on enums can never be trimmed")]
    public static FieldInfo[] GetFields(Type enumType)
    {
        return enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }
}
