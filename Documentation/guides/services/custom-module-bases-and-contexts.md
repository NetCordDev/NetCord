# Custom Module Bases

You can create custom module bases to add methods and properties to your commands, application commands and interactions. You can also apply precondition attributes on them.

## Example
[!code-cs[CustomCommandModule.cs](CustomModuleBasesAndContexts/CustomModuleBases/CustomCommandModule.cs)]

## Example Usage
[!code-cs[ExampleModule.cs](CustomModuleBasesAndContexts/CustomModuleBases/ExampleModule.cs)]

# Custom Contexts

You can create custom contexts to extend built-in contexts.

## Example
[!code-cs[CustomCommandContext.cs](CustomModuleBasesAndContexts/CustomContexts/CustomCommandContext.cs)]

## Example Usage
[!code-cs[ExampleModule.cs](CustomModuleBasesAndContexts/CustomContexts/ExampleModule.cs)]

> [!NOTE]
> Your service needs to have `CustomCommandContext` as a generic parameter to be able to load commands, application commands and interactions with the context.
