using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public abstract class Entity : IEntity
{
    public abstract ulong Id { get; }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null)
            return right is null;
        else
        {
            if (right is null)
                return false;
            else
                return left.Id == right.Id;
        }
    }

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    public override string? ToString() => Id.ToString();
    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Entity entity)
            return Id == entity.Id;
        else
            return false;
    }
}
