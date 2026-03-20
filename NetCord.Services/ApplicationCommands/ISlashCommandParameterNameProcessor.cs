using System.Text.Json;

namespace NetCord.Services.ApplicationCommands;

public interface ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration);
}

public class SlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public static SlashCommandParameterNameProcessor<TContext> Instance { get; } = new();

    private SlashCommandParameterNameProcessor()
    {
    }

    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return name;
    }
}

public class SnakeCaseSlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public static SnakeCaseSlashCommandParameterNameProcessor<TContext> Instance { get; } = new();

    private SnakeCaseSlashCommandParameterNameProcessor()
    {
    }

    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return JsonNamingPolicy.SnakeCaseLower.ConvertName(name);
    }
}

public class KebabCaseSlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public static KebabCaseSlashCommandParameterNameProcessor<TContext> Instance { get; } = new();

    private KebabCaseSlashCommandParameterNameProcessor()
    {
    }

    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return JsonNamingPolicy.KebabCaseLower.ConvertName(name);
    }
}
