# Preconditions

Preconditions determine whether a command or interaction can be invoked. They are represented by attributes. They can be applied on modules, commands, interactions and parameters.

## Built-in Preconditions

- @NetCord.Services.RequireUserPermissionsAttribute`1 - ensures that the user invoking the command or interaction has the specified permissions.
- @NetCord.Services.RequireBotPermissionsAttribute`1 - ensures that the bot has the specified permissions.
- @NetCord.Services.RequireContextAttribute`1 - ensures that the command or interaction is invoked in the specified context like Guild, GroupDM or DM.
- @NetCord.Services.RequireNsfwAttribute`1 - ensures that the command or interaction is invoked in a NSFW channel.

## Creating a Custom Precondition Attribute
[!code-cs[RequireAnimatedAvatarAttribute.cs](Preconditions/Preconditions/RequireAnimatedAvatarAttribute.cs)]

### Example usage

#### On a module
[!code-cs[ButtonModule.cs](Preconditions/Preconditions/ButtonModule.cs#L5-L9)]

#### On a command
[!code-cs[AvatarModule.cs](Preconditions/Preconditions/AvatarModule.cs#L5-L10)]

#### On a minimal API-style command
[!code-cs[Program.cs](Preconditions/Preconditions/Program.cs#L25-L28)]

## Creating a Custom Parameter Precondition Attribute
[!code-cs[MustContainAttribute.cs](Preconditions/ParameterPreconditions/MustContainAttribute.cs)]

### Example usage

#### On a command
[!code-cs[HelloModule.cs](Preconditions/ParameterPreconditions/HelloModule.cs#L5-L9)]

#### On a minimal API-style command
[!code-cs[Program.cs](Preconditions/ParameterPreconditions/Program.cs#L22-L24)]
