using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract partial class Interaction : ClientEntity, IInteraction, IJsonModel<JsonInteraction>
{
    JsonInteraction IJsonModel<JsonInteraction>.JsonModel => _jsonModel;
    private readonly JsonInteraction _jsonModel;

    private readonly InteractionResponseDelegate _sendResponseAsync;

    private protected Interaction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        if (jsonModel.GuildId is { } guildId)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId, client);
        else
            User = new User(jsonModel.User!, client);

        if (jsonModel.GuildReference is { } guildReference)
            GuildReference = new InteractionGuildReference(guildReference);

        Guild = guild;
        Channel = TextChannel.CreateFromJson(jsonModel.Channel!, client);
        
        Entitlements = jsonModel.Entitlements.Length == 0 
            ? Array.Empty<Entitlement>() 
            : jsonModel.Entitlements.Select(e => new Entitlement(e, client)).ToArray();

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
        return (jsonModel.Type, jsonModel.Data?.Type, jsonModel.Data?.ComponentType) switch
        {
            (InteractionType.ApplicationCommand, ApplicationCommandType.ChatInput, _) => new SlashCommandInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.ApplicationCommand, ApplicationCommandType.User, _)      => new UserCommandInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.ApplicationCommand, ApplicationCommandType.Message, _)   => new MessageCommandInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.ApplicationCommand, ApplicationCommandType.EntryPoint, _)=> new EntryPointCommandInteraction(jsonModel, guild, sendResponseAsync, client),
            
            (InteractionType.MessageComponent, _, ComponentType.Button)        => new ButtonInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.MessageComponent, _, ComponentType.StringMenu)    => new StringMenuInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.MessageComponent, _, ComponentType.UserMenu)      => new UserMenuInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.MessageComponent, _, ComponentType.RoleMenu)      => new RoleMenuInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.MessageComponent, _, ComponentType.MentionableMenu) => new MentionableMenuInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.MessageComponent, _, ComponentType.ChannelMenu)    => new ChannelMenuInteraction(jsonModel, guild, sendResponseAsync, client),
            
            (InteractionType.Autocomplete, _, _) => new AutocompleteInteraction(jsonModel, guild, sendResponseAsync, client),
            (InteractionType.Modal, _, _)        => new ModalInteraction(jsonModel, guild, sendResponseAsync, client),
            
            _ => throw new InvalidOperationException($"Unknown interaction structure: {jsonModel.Type}")
        };
    }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var guild = jsonModel.GuildId is { } guildId ? cache.Guilds.GetValueOrDefault(guildId) : null;
        
        return CreateFromJson(jsonModel, guild, (interaction, callback, withResponse, properties, cancellationToken) => 
            client.SendInteractionResponseAsync(interaction.Id, interaction.Token, callback, withResponse, properties, cancellationToken), client);
    }

    public Task<InteractionCallbackResponse?> SendResponseAsync(InteractionCallbackProperties callback, bool withResponse = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default) 
        => _sendResponseAsync(this, callback, withResponse, properties, cancellationToken);
}

public abstract class InteractionData(JsonInteractionData jsonModel) : IJsonModel<JsonInteractionData>
{
    JsonInteractionData IJsonModel<JsonInteractionData>.JsonModel => _jsonModel;
    private protected readonly JsonInteractionData _jsonModel = jsonModel;
}
