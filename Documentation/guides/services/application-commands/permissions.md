# Permissions

Instead of using Precondition Attributes, you can specify command required permissions to Discord. The commands will not show up to users without the permissions then. You can also specify if commands can be used in DM. Example:
[!code-cs[ExampleModule.cs](Permissions/ExampleModule.cs#L9-L19)]