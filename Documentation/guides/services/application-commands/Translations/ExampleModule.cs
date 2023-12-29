using System.Globalization;

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
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "zwierzę" },
            { new("es-ES"), "animal" }
        };
    }

    public class DescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Wysyła zwierzę, które wybrałeś" },
            { new("es-ES"), "Envía el animal que seleccionaste" }
        };
    }

    public class AnimalNameTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "zwierzę" },
            { new("es-ES"), "animal" }
        };
    }

    public class AnimalDescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Zwierzę do wysłania" },
            { new("es-ES"), "Animal para enviar" }
        };
    }

    public class AnimalDogTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Pies" },
            { new("es-ES"), "Perro/a" }
        };
    }

    public class AnimalCatTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Kot" },
            { new("es-ES"), "Gato/a" }
        };
    }

    public class AnimalFishTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Ryba" },
            { new("es-ES"), "Pez" }
        };
    }

    public class AnimalGuineaPigTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl-PL"), "Świnka morska" },
            { new("es-ES"), "Cobayo/a" }
        };
    }
}
