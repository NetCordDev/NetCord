using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildOnboardingPrompt : JsonEntity
{
    [JsonPropertyName("type")]
    public required GuildOnboardingPromptType Type { get; set; }

    [JsonPropertyName("options")]
    public required JsonGuildOnboardingPromptOption[] Options { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("single_select")]
    public required bool SingleSelect { get; set; }

    [JsonPropertyName("required")]
    public required bool Required { get; set; }

    [JsonPropertyName("in_onboarding")]
    public required bool InOnboarding { get; set; }
}
