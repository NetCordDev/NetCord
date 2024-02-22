using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("animal", "Sends the animal you selected",
        NameTranslationsProviderType = typeof(NameTranslationsProvider),
        DescriptionTranslationsProviderType = typeof(DescriptionTranslationsProvider))]
    public Task AnimalAsync(
        [SlashCommandParameter(
            Description = "Animal to send",
            NameTranslationsProviderType = typeof(AnimalNameTranslationsProvider),
            DescriptionTranslationsProviderType = typeof(AnimalDescriptionTranslationsProvider))] Animal animal)
    {
        return RespondAsync(InteractionCallback.Message(animal.ToString()));
    }

    public enum Animal
    {
        [SlashCommandChoice(NameTranslationsProviderType = typeof(AnimalDogTranslationsProvider))]
        Dog,
        [SlashCommandChoice(NameTranslationsProviderType = typeof(AnimalCatTranslationsProvider))]
        Cat,
        [SlashCommandChoice(NameTranslationsProviderType = typeof(AnimalFishTranslationsProvider))]
        Fish,
        [SlashCommandChoice(Name = "Guinea Pig", NameTranslationsProviderType = typeof(AnimalGuineaPigTranslationsProvider))]
        GuineaPig,
    }

    public class NameTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "zwierzę" },
            { "es-ES", "animal" }
        };
    }

    public class DescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Wysyła zwierzę, które wybrałeś" },
            { "es-ES", "Envía el animal que seleccionaste" }
        };
    }

    public class AnimalNameTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "zwierzę" },
            { "es-ES", "animal" }
        };
    }

    public class AnimalDescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Zwierzę do wysłania" },
            { "es-ES", "Animal para enviar" }
        };
    }

    public class AnimalDogTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Pies" },
            { "es-ES", "Perro/a" }
        };
    }

    public class AnimalCatTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Kot" },
            { "es-ES", "Gato/a" }
        };
    }

    public class AnimalFishTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Ryba" },
            { "es-ES", "Pez" }
        };
    }

    public class AnimalGuineaPigTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<string, string>? Translations => new Dictionary<string, string>
        {
            { "pl", "Świnka morska" },
            { "es-ES", "Cobayo/a" }
        };
    }
}
