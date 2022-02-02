using System.Diagnostics.CodeAnalysis;

namespace NetCord;

internal class ReferenceEqualityComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
    public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}