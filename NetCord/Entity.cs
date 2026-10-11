using System.Diagnostics.CodeAnalysis;

using NetCord.JsonModels;

namespace NetCord;

public abstract class Entity(ulong id) : IEntity, ISpanFormattable, IEquatable<Entity>
{
    public Entity(JsonEntity jsonModel) : this(jsonModel.Id)
    {
    }

    public ulong Id { get; } = id;

    public DateTimeOffset CreatedAt => Snowflake.Timestamp(Id);

    public static bool operator ==(Entity? left, Entity? right) => left is null ? right is null : right is not null && left.Id == right.Id;

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Entity entity && Id == entity.Id;

    public bool Equals([NotNullWhen(true)] Entity? other) => other is not null && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Id.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public virtual bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Id.TryFormat(destination, out charsWritten);
}
