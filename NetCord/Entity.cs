using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public abstract class Entity : IEntity
{
    public abstract ulong Id { get; }

    public DateTimeOffset CreatedAt => Snowflake.CreatedAt(Id);

    public static bool operator ==(Entity? left, Entity? right) => left is null ? right is null : right is not null && left.Id == right.Id;

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Entity entity && Id == entity.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Id.ToString();
}
