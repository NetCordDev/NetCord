using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract partial class Interaction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ClientEntity(client), IInteraction
{
    public override ulong Id { get; } = jsonModel.Id;

    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public InteractionGuildReference? GuildReference { get; } = jsonModel.GuildReference is { } guildReference ? new(guildReference) : null;

    public Guild? Guild { get; } = guild;

    public TextChannel Channel { get; } = TextChannel.CreateFromJson(jsonModel.Channel!, client);

    public User User { get; } = jsonModel.GuildId is { } guildId ? new GuildInteractionUser(jsonModel.GuildUser!, guildId, client) : new User(jsonModel.User!, client);

    public string Token { get; } = jsonModel.Token;

    public int Version { get; } = jsonModel.Version;

    public Permissions AppPermissions { get; } = jsonModel.AppPermissions;

    public string Locale { get; } = jsonModel.Locale!;

    public string? GuildLocale { get; } = jsonModel.GuildLocale;

    public IReadOnlyList<Entitlement> Entitlements { get; } = [.. jsonModel.Entitlements.Select(e => new Entitlement(e, client))];

    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; } = jsonModel.AuthorizingIntegrationOwners;

    public InteractionContextType Context { get; } = jsonModel.Context.GetValueOrDefault();

    public long AttachmentSizeLimit { get; } = jsonModel.AttachmentSizeLimit;

    public abstract InteractionData Data { get; }

    public static Interaction Create(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client)
    {
        return jsonModel switch
        {
            JsonApplicationCommandInteraction applicationCommandInteraction => applicationCommandInteraction.Data.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(applicationCommandInteraction, guild, sendResponseAsync, client),
                ApplicationCommandType.User => new UserCommandInteraction(applicationCommandInteraction, guild, sendResponseAsync, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(applicationCommandInteraction, guild, sendResponseAsync, client),
                ApplicationCommandType.EntryPoint => new EntryPointCommandInteraction(applicationCommandInteraction, guild, sendResponseAsync, client),
                _ => new UnknownApplicationCommandInteraction(applicationCommandInteraction, guild, sendResponseAsync, client),
            },
            JsonMessageComponentInteraction messageComponentInteraction => messageComponentInteraction.Data.Type switch
            {
                ComponentType.Button => new ButtonInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                ComponentType.StringSelect => new StringSelectInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                ComponentType.UserSelect => new UserSelectInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                ComponentType.RoleSelect => new RoleSelectInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                ComponentType.MentionableSelect => new MentionableSelectInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                ComponentType.ChannelSelect => new ChannelSelectInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
                _ => new UnknownMessageComponentInteraction(messageComponentInteraction, guild, sendResponseAsync, client),
            },
            JsonAutocompleteInteraction autocompleteInteraction => new AutocompleteInteraction(autocompleteInteraction, guild, sendResponseAsync, client),
            JsonModalSubmitInteraction modalInteraction => new ModalSubmitInteraction(modalInteraction, guild, sendResponseAsync, client),
            _ => new UnknownInteraction(jsonModel, guild, sendResponseAsync, client),
        };
    }

    public static Interaction Create(JsonInteraction jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var guild = jsonModel.GuildId is { } guildId
            ? cache.Guilds.GetValueOrDefault(guildId)
            : null;

        return Create(jsonModel,
                      guild,
                      (interaction, callback, withResponse, properties, cancellationToken) =>
                      {
                          return client.SendInteractionResponseAsync(interaction.Id, interaction.Token, callback, withResponse, properties, cancellationToken);
                      },
                      client);
    }

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
    }
}

public abstract class InteractionData;

public sealed class UnknownInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    private static readonly UnknownInteractionData s_data = new();

    public override UnknownInteractionData Data => s_data;
}

public sealed class UnknownInteractionData : InteractionData;
