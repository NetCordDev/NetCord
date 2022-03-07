﻿namespace NetCord;

public abstract class Entity : IEntity
{
    public abstract DiscordId Id { get; }

    public static bool operator ==(Entity? left, Entity? right) => left?.Id == right?.Id;

    public static bool operator !=(Entity? left, Entity? right) => !(left?.Id == right?.Id);

    public override string? ToString() => Id.ToString();
    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals(object? obj) => obj is Entity entity && Id == entity.Id;

    public static implicit operator DiscordId(Entity entity) => entity.Id;
}