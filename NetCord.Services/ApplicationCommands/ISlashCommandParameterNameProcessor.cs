using System.Text.Json;

namespace NetCord.Services.ApplicationCommands;

public interface ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration);
}

public class SlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return name;
    }
}

public class SnakeCaseSlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return JsonNamingPolicy.SnakeCaseLower.ConvertName(name);
    }
}

public class KebabCaseSlashCommandParameterNameProcessor<TContext> : ISlashCommandParameterNameProcessor<TContext> where TContext : IApplicationCommandContext
{
    public string ProcessParameterName(string name, ApplicationCommandServiceConfiguration<TContext> configuration)
    {
        return JsonNamingPolicy.KebabCaseLower.ConvertName(name);
    }
}
