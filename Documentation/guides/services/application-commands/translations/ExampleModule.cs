using System.Globalization;

using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("animal", "Sends the animal you selected", NameTranslationsProviderType = typeof(NameTranslationsProvider), DescriptionTranslationsProviderType = typeof(DescriptionTranslationsProvider))]
    public Task AnimalAsync(
        [SlashCommandParameter(Description = "Animal to send", NameTranslationsProviderType = typeof(AnimalNameTranslationsProvider), DescriptionTranslationsProviderType = typeof(AnimalDescriptionTranslationsProvider))] Animal animal)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(animal.ToString()));
    }

    public enum Animal
    {
        [SlashCommandChoice(TranslationsProviderType = typeof(AnimalDogTranslationsProvider))]
        Dog,
        [SlashCommandChoice(TranslationsProviderType = typeof(AnimalCatTranslationsProvider))]
        Cat,
        [SlashCommandChoice(TranslationsProviderType = typeof(AnimalFishTranslationsProvider))]
        Fish,
        [SlashCommandChoice(Name = "Guinea Pig", TranslationsProviderType = typeof(AnimalGuineaPigTranslationsProvider))]
        GuineaPig,
    }

    public class NameTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "zwierzę" },
            { new("es-ES"), "animal" }
        };
    }

    public class DescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Wysyła zwierzę, które wybrałeś" },
            { new("es-ES"), "Envía el animal que seleccionaste" }
        };
    }

    public class AnimalNameTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "zwierzę" },
            { new("es-ES"), "animal" }
        };
    }

    public class AnimalDescriptionTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Zwierzę do wysłania" },
            { new("es-ES"), "Animal para enviar" }
        };
    }

    public class AnimalDogTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Pies" },
            { new("es-ES"), "Perro/a" }
        };
    }

    public class AnimalCatTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Kot" },
            { new("es-ES"), "Gato/a" }
        };
    }

    public class AnimalFishTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Ryba" },
            { new("es-ES"), "Pez" }
        };
    }

    public class AnimalGuineaPigTranslationsProvider : ITranslationsProvider
    {
        public IReadOnlyDictionary<CultureInfo, string>? Translations => new Dictionary<CultureInfo, string>
        {
            { new("pl"), "Świnka morska" },
            { new("es-ES"), "Cobayo/a" }
        };
    }
}
