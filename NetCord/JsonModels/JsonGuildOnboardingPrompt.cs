using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public class JsonGuildOnboardingPrompt : JsonEntity
{
    [JsonPropertyName("type")]
    public GuildOnboardingPromptType Type { get; set; }

    [JsonPropertyName("options")]
    public JsonGuildOnboardingPromptOption[] Options { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("single_select")]
    public bool SingleSelect { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("in_onboarding")]
    public bool InOnboarding { get; set; }
}
