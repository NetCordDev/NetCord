using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract partial class Interaction : ClientEntity, IInteraction
{
    JsonInteraction IJsonModel<JsonInteraction>.JsonModel => _jsonModel;
    private readonly JsonInteraction _jsonModel;

    private readonly InteractionResponseDelegate _sendResponseAsync;

    private protected Interaction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        var guildReference = jsonModel.GuildReference;
        if (guildReference is not null)
            GuildReference = new(guildReference);

        Guild = guild;
        Channel = TextChannel.CreateFromJson(jsonModel.Channel!, client);
        Entitlements = jsonModel.Entitlements.Select(e => new Entitlement(e, client)).ToArray();

        _sendResponseAsync = sendResponseAsync;
    }

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public ulong? GuildId => _jsonModel.GuildId;

    public InteractionGuildReference? GuildReference { get; }

    public Guild? Guild { get; }

    public TextChannel Channel { get; }

    public User User { get; }

    public string Token => _jsonModel.Token;

    public int Version => _jsonModel.Version;

    public Permissions AppPermissions => _jsonModel.AppPermissions;

    public string UserLocale => _jsonModel.UserLocale!;

    public string? GuildLocale => _jsonModel.GuildLocale;

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners => _jsonModel.AuthorizingIntegrationOwners!;

    public InteractionContextType Context => _jsonModel.Context.GetValueOrDefault();

    public long AttachmentSizeLimit => _jsonModel.AttachmentSizeLimit;

    public abstract InteractionData Data { get; }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client)
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

    public static Interaction CreateFromJson(JsonInteraction jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var guildId = jsonModel.GuildId;
        var guild = guildId.HasValue ? cache.Guilds.GetValueOrDefault(guildId.GetValueOrDefault()) : null;
        return CreateFromJson(jsonModel, guild, (interaction, callback, withResponse, properties, cancellationToken) => client.SendInteractionResponseAsync(interaction.Id, interaction.Token, callback, withResponse, properties, cancellationToken), client);
    }

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) => _sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
}

public abstract class InteractionData;

public sealed class UnknownInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    private static readonly UnknownInteractionData s_data = new();

    public override UnknownInteractionData Data => s_data;
}

public sealed class UnknownInteractionData : InteractionData;
