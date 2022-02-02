﻿namespace NetCord;

public class ButtonInteractionData : InteractionData
{
    public string CustomId => _jsonEntity.CustomId!;

    public ComponentType ComponentType => _jsonEntity.ComponentType.GetValueOrDefault();

    internal ButtonInteractionData(JsonModels.JsonInteractionData jsonEntity) : base(jsonEntity)
    {
    }
}