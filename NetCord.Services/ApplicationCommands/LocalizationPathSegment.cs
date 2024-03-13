using System.Reflection;

namespace NetCord.Services.ApplicationCommands;

public abstract class LocalizationPathSegment;

public class ApplicationCommandLocalizationPathSegment(string name) : LocalizationPathSegment
{
    public string Name { get; } = name;
}

public class SlashCommandGroupLocalizationPathSegment(string name) : LocalizationPathSegment
{
    public string Name { get; } = name;
}

public class SubSlashCommandLocalizationPathSegment(string name) : LocalizationPathSegment
{
    public string Name { get; } = name;
}

public class SubSlashCommandGroupLocalizationPathSegment(string name) : LocalizationPathSegment
{
    public string Name { get; } = name;
}

public class SlashCommandParameterLocalizationPathSegment(string name) : LocalizationPathSegment
{
    public string Name { get; } = name;
}

public class EnumLocalizationPathSegment(Type type) : LocalizationPathSegment
{
    public Type Type { get; } = type;
}

public class EnumFieldLocalizationPathSegment(FieldInfo field) : LocalizationPathSegment
{
    public FieldInfo Field { get; } = field;
}

public class NameLocalizationPathSegment : LocalizationPathSegment
{
    protected NameLocalizationPathSegment()
    {
    }

    public static NameLocalizationPathSegment Instance { get; } = new();
}

public class DescriptionLocalizationPathSegment : LocalizationPathSegment
{
    protected DescriptionLocalizationPathSegment()
    {
    }

    public static DescriptionLocalizationPathSegment Instance { get; } = new();
}
