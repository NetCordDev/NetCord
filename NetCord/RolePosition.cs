using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public readonly struct RolePosition : IComparable<RolePosition>, IEquatable<RolePosition>
{
    private readonly int _position;
    private readonly ulong _roleId;

    internal RolePosition(int position, ulong roleId)
    {
        _position = position;
        _roleId = roleId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_position, _roleId);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is RolePosition other && Equals(other);
    }

    public bool Equals(RolePosition other)
    {
        return _position == other._position && _roleId == other._roleId;
    }

    public int CompareTo(RolePosition other)
    {
        var positionCompare = _position.CompareTo(other._position);
        return positionCompare is 0 ? other._roleId.CompareTo(_roleId) : positionCompare;
    }

    public static bool operator ==(RolePosition left, RolePosition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RolePosition left, RolePosition right)
    {
        return !left.Equals(right);
    }

    public static bool operator >(RolePosition left, RolePosition right)
    {
        return left._position > right._position || (left._position == right._position && left._roleId < right._roleId);
    }

    public static bool operator <(RolePosition left, RolePosition right)
    {
        return left._position < right._position || (left._position == right._position && left._roleId > right._roleId);
    }

    public static bool operator >=(RolePosition left, RolePosition right)
    {
        return left._position > right._position || (left._position == right._position && left._roleId <= right._roleId);
    }

    public static bool operator <=(RolePosition left, RolePosition right)
    {
        return left._position < right._position || (left._position == right._position && left._roleId >= right._roleId);
    }
}
