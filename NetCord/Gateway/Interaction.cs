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

    public ulong? ChannelId => _jsonModel.ChannelId;

    public TextChannel? Channel { get; }

    public User User { get; }

    public string Token => _jsonModel.Token;

    public Permissions? AppPermissions => _jsonModel.AppPermissions;

    public CultureInfo UserLocale => _jsonModel.UserLocale;

    public CultureInfo? GuildLocale => _jsonModel.GuildLocale;

    public abstract InteractionData Data { get; }

    private protected Interaction(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.GuildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, jsonModel.GuildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);

        Guild = guild;
        Channel = channel;
    }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, Guild? guild, TextChannel? channel, RestClient client)
    {
        return jsonModel.Type switch
        {
            InteractionType.ApplicationCommand => jsonModel.Data!.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonModel, guild, channel, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonModel, guild, channel, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonModel, guild, channel, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonModel.Data!.ComponentType switch
            {
                ComponentType.Button => new ButtonInteraction(jsonModel, guild, channel, client),
                ComponentType.StringMenu => new StringMenuInteraction(jsonModel, guild, channel, client),
                ComponentType.UserMenu => new UserMenuInteraction(jsonModel, guild, channel, client),
                ComponentType.RoleMenu => new RoleMenuInteraction(jsonModel, guild, channel, client),
                ComponentType.MentionableMenu => new MentionableMenuInteraction(jsonModel, guild, channel, client),
                ComponentType.ChannelMenu => new ChannelMenuInteraction(jsonModel, guild, channel, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.ApplicationCommandAutocomplete => new ApplicationCommandAutocompleteInteraction(jsonModel, guild, channel, client),
            InteractionType.ModalSubmit => new ModalSubmitInteraction(jsonModel, guild, channel, client),
            _ => throw new InvalidOperationException(),
        };
    }

    public static Interaction CreateFromJson(JsonInteraction jsonModel, GatewayClient client)
    {
        Guild? guild;
        TextChannel? channel;
        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
        {
            if (client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out guild))
            {
                var channelId = jsonModel.ChannelId;
                if (channelId.HasValue)
                {
                    var channelIdValue = channelId.GetValueOrDefault();
                    if (guild.Channels.TryGetValue(channelIdValue, out var guildChannel))
                        channel = (TextChannel)guildChannel;
                    else if (guild.ActiveThreads.TryGetValue(channelIdValue, out var thread))
                        channel = thread;
                    else
                        channel = null;
                }
                else
                    channel = null;
            }
            else
                channel = null;
        }
        else
        {
            guild = null;
            var channelId = jsonModel.ChannelId;
            if (channelId.HasValue)
            {
                var channelIdValue = channelId.GetValueOrDefault();
                if (client.DMChannels.TryGetValue(channelIdValue, out var dMChannel))
                    channel = dMChannel;
                else if (client.GroupDMChannels.TryGetValue(channelIdValue, out var groupDMChannel))
                    channel = groupDMChannel;
                else
                    channel = null;
            }
            else
                channel = null;
        }
        return CreateFromJson(jsonModel, guild, channel, client.Rest);
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
