using System.Globalization;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public abstract class Interaction : ClientEntity, IJsonModel<JsonInteraction>
{
    JsonInteraction IJsonModel<JsonInteraction>.JsonModel => _jsonModel;
    private readonly JsonInteraction _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public InteractionType Type => _jsonModel.Type;

    public ulong? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; }

    public TextChannel Channel { get; }

    public User User { get; }

    public string Token => _jsonModel.Token;

    public Permissions? AppPermissions => _jsonModel.AppPermissions;

    public CultureInfo UserLocale => _jsonModel.UserLocale!;

    public CultureInfo? GuildLocale => _jsonModel.GuildLocale;

    public abstract InteractionData Data { get; }

    private protected Interaction(JsonInteraction jsonModel, Guild? guild, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.GuildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, jsonModel.GuildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        Guild = guild;
        Channel = (TextChannel)NetCord.Channel.CreateFromJson(jsonModel.Channel!, client);
    }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, Guild? guild, RestClient client)
    {
        return jsonModel.Type switch
        {
            InteractionType.ApplicationCommand => jsonModel.Data!.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonModel, guild, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonModel, guild, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonModel, guild, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonModel.Data!.ComponentType switch
            {
                ComponentType.Button => new ButtonInteraction(jsonModel, guild, client),
                ComponentType.StringMenu => new StringMenuInteraction(jsonModel, guild, client),
                ComponentType.UserMenu => new UserMenuInteraction(jsonModel, guild, client),
                ComponentType.RoleMenu => new RoleMenuInteraction(jsonModel, guild, client),
                ComponentType.MentionableMenu => new MentionableMenuInteraction(jsonModel, guild, client),
                ComponentType.ChannelMenu => new ChannelMenuInteraction(jsonModel, guild, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.ApplicationCommandAutocomplete => new ApplicationCommandAutocompleteInteraction(jsonModel, guild, client),
            InteractionType.ModalSubmit => new ModalSubmitInteraction(jsonModel, guild, client),
            _ => throw new InvalidOperationException(),
        };
    }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, IGatewayClientCache cache, RestClient client)
    {
        var guildId = jsonModel.GuildId;
        var guild = guildId.HasValue ? cache.Guilds.GetValueOrDefault(guildId.GetValueOrDefault()) : null;
        return CreateFromJson(jsonModel, guild, client);
    }

    public Task SendResponseAsync(InteractionCallback interactionCallback, RequestProperties? properties = null) => _client.SendInteractionResponseAsync(Id, Token, interactionCallback, properties);

    public Task<RestMessage> GetResponseAsync(RequestProperties? properties = null) => _client.GetInteractionResponseAsync(ApplicationId, Token, properties);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyInteractionResponseAsync(ApplicationId, Token, action, properties);

    public Task DeleteResponseAsync(RequestProperties? properties = null) => _client.DeleteInteractionResponseAsync(ApplicationId, Token, properties);

    public Task<RestMessage> SendFollowupMessageAsync(InteractionMessageProperties message, RequestProperties? properties = null) => _client.SendInteractionFollowupMessageAsync(ApplicationId, Token, message, properties);

    public Task<RestMessage> GetFollowupMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.GetInteractionFollowupMessageAsync(ApplicationId, Token, messageId, properties);

    public Task<RestMessage> ModifyFollowupMessageAsync(ulong messageId, Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyInteractionFollowupMessageAsync(ApplicationId, Token, messageId, action, properties);

    public Task DeleteFollowupMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.DeleteInteractionFollowupMessageAsync(ApplicationId, Token, messageId, properties);
}
