# Localizations

To localize application commands, you need to use @NetCord.Services.ApplicationCommands.ILocalizationsProvider. You specify it in the configuration.

## Specifying the localizations provider

The samples below show how to specify the @NetCord.Services.ApplicationCommands.JsonLocalizationsProvider.

## [.NET Generic Host](#tab/generic-host)
[!code-cs[Program.cs](LocalizationsHosting/Program.cs?highlight=4#L11-L15)]

## [Bare Bones](#tab/bare-bones)
[!code-cs[Program.cs](Localizations/Program.cs?highlight=3#L12-L15)]

***

## Localizing commands with the @NetCord.Services.ApplicationCommands.JsonLocalizationsProvider

The @NetCord.Services.ApplicationCommands.JsonLocalizationsProvider employs JSON files to localize commands. By default, these JSON files are expected to reside in the `Localizations` directory, with each file named after its respective locale in the format `{locale}.json`. For instance, a Polish localization file might be named `pl.json`. A list of supported locales can be found [here](https://discord.com/developers/docs/reference#locales).

### Example

The file below shows Polish localization of `permissions` and `animal` commands.

[!code-json[pl.json](Localizations/Localizations/pl.json)]

Here are the commands.

[!code-cs[AnimalModule.cs](Localizations/AnimalModule.cs)]

[!code-cs[PermissionsModule.cs](Localizations/PermissionsModule.cs)]
