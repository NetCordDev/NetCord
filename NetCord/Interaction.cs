using System.Globalization;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public abstract class Interaction : ClientEntity, IInteraction
{
    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonInteraction _jsonModel;

    private readonly Func<IInteraction, InteractionCallback, RequestProperties?, Task> _sendResponseAsync;

    private protected Interaction(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        Guild = guild;
        Channel = TextChannel.CreateFromJson(jsonModel.Channel!, client);
        Entitlements = jsonModel.Entitlements.Select(e => new Entitlement(e)).ToArray();

        _sendResponseAsync = sendResponseAsync;
    }

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public ulong? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; }

    public TextChannel Channel { get; }

    public User User { get; }

    public string Token => _jsonModel.Token;

    public Permissions? AppPermissions => _jsonModel.AppPermissions;

    public CultureInfo UserLocale => _jsonModel.UserLocale!;

    public CultureInfo? GuildLocale => _jsonModel.GuildLocale;

    public IReadOnlyList<Entitlement> Entitlements { get; }

    public abstract InteractionData Data { get; }

    public static Interaction CreateFromJson(JsonModels.JsonInteraction jsonModel, Guild? guild, Func<IInteraction, InteractionCallback, RequestProperties?, Task> sendResponseAsync, RestClient client)
    {
        return jsonModel.Type switch
        {
            InteractionType.ApplicationCommand => jsonModel.Data!.Type.GetValueOrDefault() switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonModel, guild, sendResponseAsync, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonModel, guild, sendResponseAsync, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonModel, guild, sendResponseAsync, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonModel.Data!.ComponentType.GetValueOrDefault() switch
            {
                ComponentType.Button => new ButtonInteraction(jsonModel, guild, sendResponseAsync, client),
                ComponentType.StringMenu => new StringMenuInteraction(jsonModel, guild, sendResponseAsync, client),
                ComponentType.UserMenu => new UserMenuInteraction(jsonModel, guild, sendResponseAsync, client),
                ComponentType.RoleMenu => new RoleMenuInteraction(jsonModel, guild, sendResponseAsync, client),
                ComponentType.MentionableMenu => new MentionableMenuInteraction(jsonModel, guild, sendResponseAsync, client),
                ComponentType.ChannelMenu => new ChannelMenuInteraction(jsonModel, guild, sendResponseAsync, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.ApplicationCommandAutocomplete => new AutocompleteInteraction(jsonModel, guild, sendResponseAsync, client),
            InteractionType.ModalSubmit => new ModalSubmitInteraction(jsonModel, guild, sendResponseAsync, client),
            _ => throw new InvalidOperationException(),
        };
    }

    public static Interaction CreateFromJson(JsonModels.JsonInteraction jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var guildId = jsonModel.GuildId;
        var guild = guildId.HasValue ? cache.Guilds.GetValueOrDefault(guildId.GetValueOrDefault()) : null;
        return CreateFromJson(jsonModel, guild, (interaction, interactionCallback, properties) => client.SendInteractionResponseAsync(interaction.Id, interaction.Token, interactionCallback, properties), client);
    }

    public Task SendResponseAsync(InteractionCallback interactionCallback, RequestProperties? properties = null) => _sendResponseAsync(this, interactionCallback, properties);

    public Task<RestMessage> GetResponseAsync(RequestProperties? properties = null) => _client.GetInteractionResponseAsync(ApplicationId, Token, properties);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyInteractionResponseAsync(ApplicationId, Token, action, properties);

    public Task DeleteResponseAsync(RequestProperties? properties = null) => _client.DeleteInteractionResponseAsync(ApplicationId, Token, properties);

    public Task<RestMessage> SendFollowupMessageAsync(InteractionMessageProperties message, RequestProperties? properties = null) => _client.SendInteractionFollowupMessageAsync(ApplicationId, Token, message, properties);

    public Task<RestMessage> GetFollowupMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.GetInteractionFollowupMessageAsync(ApplicationId, Token, messageId, properties);

    public Task<RestMessage> ModifyFollowupMessageAsync(ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyInteractionFollowupMessageAsync(ApplicationId, Token, messageId, action, properties);

    public Task DeleteFollowupMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.DeleteInteractionFollowupMessageAsync(ApplicationId, Token, messageId, properties);
}
