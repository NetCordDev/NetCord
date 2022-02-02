using NetCord.Services.SlashCommands;

namespace NetCord.Test.SlashCommands;

internal class DeleteMessagesDaysTypeReader : SlashCommandTypeReader<SlashCommandContext>
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, SlashCommandContext context, SlashCommandParameter<SlashCommandContext> parameter, SlashCommandServiceOptions<SlashCommandContext> options)
    {
        return Task.FromResult((object)(DeleteMessagesDays)int.Parse(value));
    }

    public override IEnumerable<ApplicationCommandOptionChoiceProperties> GetChoices(SlashCommandParameter<SlashCommandContext> parameter)
    {
        return new List<ApplicationCommandOptionChoiceProperties>()
        {
            new("Don't remove", (double)DeleteMessagesDays.DontRemove),
            new("Last 24 hours", (double)DeleteMessagesDays.Last24Hours),
            new("Last 2 days", (double)DeleteMessagesDays.Last2Days),
            new("Last 3 days", (double)DeleteMessagesDays.Last3Days),
            new("Last 4 days", (double)DeleteMessagesDays.Last4Days),
            new("Last 5 days", (double)DeleteMessagesDays.Last5Days),
            new("Last 6 days", (double)DeleteMessagesDays.Last6Days),
            new("Last week", (double)DeleteMessagesDays.LastWeek),
        };
    }
}