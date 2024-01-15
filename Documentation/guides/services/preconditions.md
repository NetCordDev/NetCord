# Preconditions

Preconditions determine whether a command or interaction can be invoked. They are represented by attributes. They can be applied on Modules, Commands and Parameters.

## Built-in Precondition Attributes

> [!NOTE]
> The attributes are generic. Generic attributes are supported since C# 11.

- @NetCord.Services.RequireUserPermissionsAttribute`1
- @NetCord.Services.RequireBotPermissionsAttribute`1
- @NetCord.Services.RequireContextAttribute`1
- @NetCord.Services.RequireNsfwAttribute`1

## Creating a Custom Precondition Attribute
[!code-cs[RequireDiscriminatorAttribute.cs](Preconditions/Preconditions/RequireDiscriminatorAttribute.cs)]

### Example usage
[!code-cs[ExampleModule.cs](Preconditions/Preconditions/ExampleModule.cs)]

## Creating a Custom Parameter Precondition Attribute
[!code-cs[MustContainAttribute.cs](Preconditions/ParameterPreconditions/MustContainAttribute.cs)]

### Example usage
[!code-cs[ExampleModule.cs](Preconditions/ParameterPreconditions/ExampleModule.cs)]