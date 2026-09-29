namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Specifies that an enum value should be ignored and not shown as an option in slash commands.
/// Apply this attribute to enum fields that should not be presented to users as choices.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SlashCommandIgnoreAttribute : Attribute
{
}
