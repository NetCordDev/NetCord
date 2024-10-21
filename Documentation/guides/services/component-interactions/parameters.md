# Parameters

Component interaction parameters allow you to pass data to components during their creation. For example, you can pass a user ID to ban or the content of a message to send. By default, parameters are separated by a colon (`:`), but this behavior can be customized by setting @NetCord.Services.ComponentInteractions.ComponentInteractionServiceConfiguration`1.ParameterSeparator in the configuration. This guide assumes the default separator is used.

## Type Readers

NetCord uses type readers to convert parameters to their respective types. The following table lists the default type readers and the types they read.

<details>
<summary>Expand to see the full list of built-in type readers.</summary>

| Type Reader                                                                          | Type Read                                                      |
|--------------------------------------------------------------------------------------|----------------------------------------------------------------|
| @NetCord.Services.ComponentInteractions.TypeReaders.BigIntegerTypeReader`1           | @System.Numerics.BigInteger                                    |
| @NetCord.Services.ComponentInteractions.TypeReaders.BooleanTypeReader`1              | @System.Boolean                                                |
| @NetCord.Services.ComponentInteractions.TypeReaders.ByteTypeReader`1                 | @System.Byte                                                   |
| @NetCord.Services.ComponentInteractions.TypeReaders.CharTypeReader`1                 | @System.Char                                                   |
| @NetCord.Services.ComponentInteractions.TypeReaders.CodeBlockTypeReader`1            | @NetCord.CodeBlock                                             |
| @NetCord.Services.ComponentInteractions.TypeReaders.DateOnlyTypeReader`1             | @System.DateOnly                                               |
| @NetCord.Services.ComponentInteractions.TypeReaders.DateTimeOffsetTypeReader`1       | @System.DateTimeOffset                                         |
| @NetCord.Services.ComponentInteractions.TypeReaders.DateTimeTypeReader`1             | @System.DateTime                                               |
| @NetCord.Services.ComponentInteractions.TypeReaders.DecimalTypeReader`1              | @System.Decimal                                                |
| @NetCord.Services.ComponentInteractions.TypeReaders.DoubleTypeReader`1               | @System.Double                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.EnumTypeReader`1                 | @System.Enum                                                   |
| @NetCord.Services.ComponentInteractions.TypeReaders.GuildUserTypeReader`1            | @NetCord.GuildUser                                             |
| @NetCord.Services.ComponentInteractions.TypeReaders.HalfTypeReader`1                 | @System.Half                                                   |
| @NetCord.Services.ComponentInteractions.TypeReaders.Int128TypeReader`1               | @System.Int128                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.Int16TypeReader`1                | @System.Int16                                                  |
| @NetCord.Services.ComponentInteractions.TypeReaders.Int32TypeReader`1                | @System.Int32                                                  |
| @NetCord.Services.ComponentInteractions.TypeReaders.Int64TypeReader`1                | @System.Int64                                                  |
| @NetCord.Services.ComponentInteractions.TypeReaders.IntPtrTypeReader`1               | @System.IntPtr                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.ReadOnlyMemoryOfCharTypeReader`1 | @"System.ReadOnlyMemory`1?text=ReadOnlyMemory"<@"System.Char"> |
| @NetCord.Services.ComponentInteractions.TypeReaders.SByteTypeReader`1                | @System.SByte                                                  |
| @NetCord.Services.ComponentInteractions.TypeReaders.SingleTypeReader`1               | @System.Single                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.StringTypeReader`1               | @System.String                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.TimeOnlyTypeReader`1             | @System.TimeOnly                                               |
| @NetCord.Services.ComponentInteractions.TypeReaders.TimeSpanTypeReader`1             | @System.TimeSpan                                               |
| @NetCord.Services.ComponentInteractions.TypeReaders.TimestampTypeReader`1            | @NetCord.Timestamp                                             |
| @NetCord.Services.ComponentInteractions.TypeReaders.UInt128TypeReader`1              | @System.UInt128                                                |
| @NetCord.Services.ComponentInteractions.TypeReaders.UInt16TypeReader`1               | @System.UInt16                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.UInt32TypeReader`1               | @System.UInt32                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.UInt64TypeReader`1               | @System.UInt64                                                 |
| @NetCord.Services.ComponentInteractions.TypeReaders.UIntPtrTypeReader`1              | @System.UIntPtr                                                |
| @NetCord.Services.ComponentInteractions.TypeReaders.UriTypeReader`1                  | @System.Uri                                                    |
| @NetCord.Services.ComponentInteractions.TypeReaders.UserIdTypeReader`1               | @NetCord.Services.UserId                                       |

</details>

<br />

## Remainder

The last parameter is always considered a remainder. This means that it can contain any number of colons. This is useful when you want to pass a string that may contain colons.

For example, using a custom ID like `publish:testing 1 2 3` would send an embed with the content `testing 1 2 3`.

[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L9-L16)]

## Variable Number of Parameters

You can use the `params` keyword to accept a variable number of parameters.

For example, a custom ID like `delete:931274046312701962:963913427661766717` would delete messages with the IDs `931274046312701962` and `963913427661766717`.

[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L18-L24)]

## Optional Parameters

To mark parameters as optional, assign them a default value.

For example, the following custom IDs can be used:
- `unban:735048387178659854:` (note the trailing `:` to indicate the omission of the `reason` parameter) to unban the user without specifying a reason.
- `unban:735048387178659854:proof of innocence` to unban the user and provide a reason.

[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L26-L32)]

In another example, with two optional parameters, you can use these custom IDs:
- `bug report:troll::` (note the two consecutive colons `::` at the end to indicate the omission of both `body` and `category` parameters) to create a bug report titled `troll` without a body or category.
- `bug report:Channel Disappeared:I can no longer find the general channel:` (note the trailing colon `:` to indicate the omission of the `category` parameter) to create a bug report with a title and body, but no category.
- `bug report:Bot Bug::Bot` (note the two consecutive colons `::` in the middle to indicate the omission of the `body` parameter) to create a bug report with a title and category but no body.
- `bug report:Wrong Channel Permissions:I am able to send messages in the announcements channel:Permissions` to create a bug report with all parameters specified.

[!code-cs[ExampleModule.cs](Parameters/ExampleModule.cs#L34-L43)]
