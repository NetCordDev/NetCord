using System.Globalization;

using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public abstract class Interaction : ClientEntity, IJsonModel<JsonInteraction>
{
    JsonInteraction IJsonModel<JsonInteraction>.JsonModel => _jsonModel;
    private readonly JsonInteraction _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake ApplicationId => _jsonModel.ApplicationId;

    public InteractionType Type => _jsonModel.Type;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public Guild? Guild { get; }

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public TextChannel? Channel { get; }

    public User User { get; }

    public string Token => _jsonModel.Token;

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
            InteractionType.ApplicationCommand => jsonModel.Data.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonModel, guild, channel, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonModel, guild, channel, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonModel, guild, channel, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonModel.Data.ComponentType switch
            {
                ComponentType.Button => new ButtonInteraction(jsonModel, guild, channel, client),
                ComponentType.Menu => new MenuInteraction(jsonModel, guild, channel, client),
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
                if (jsonModel.ChannelId.HasValue)
                {
                    if (guild.Channels.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var guildChannel))
                        channel = (TextChannel)guildChannel;
                    else if (guild.ActiveThreads.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var thread))
                        channel = thread;
                    else
                        channel = null;
                }
                else
                    channel = null;
            }
            else if (jsonModel.ChannelId.HasValue)
            {
                guild = null;
                if (client.DMChannels.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var dMChannel))
                    channel = dMChannel;
                else if (client.GroupDMChannels.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var groupDMChannel))
                    channel = groupDMChannel;
                else
                    channel = null;
            }
            else
            {
                guild = null;
                channel = null;
            }
        }
        else
        {
            guild = null;
            if (jsonModel.ChannelId.HasValue)
            {
                if (client.DMChannels.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var dMChannel))
                    channel = dMChannel;
                else if (client.GroupDMChannels.TryGetValue(jsonModel.ChannelId.GetValueOrDefault(), out var groupDMChannel))
                    channel = groupDMChannel;
                else
                    channel = null;
            }
            else
                channel = null;
        }
        return CreateFromJson(jsonModel, guild, channel, client.Rest);
    }

    public Task SendResponseAsync(InteractionCallback interactionCallback, RequestProperties? options = null) => _client.SendInteractionResponseAsync(Id, Token, interactionCallback, options);

    public Task<RestMessage> GetResponseAsync(RequestProperties? options = null) => _client.GetInteractionResponseAsync(ApplicationId, Token, options);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RequestProperties? options = null) => _client.ModifyInteractionResponseAsync(ApplicationId, Token, action, options);

    public Task DeleteResponseAsync(RequestProperties? options = null) => _client.DeleteInteractionResponseAsync(ApplicationId, Token, options);

    public Task<RestMessage> SendFollowupMessageAsync(InteractionMessageProperties message, RequestProperties? options = null) => _client.SendInteractionFollowupMessageAsync(ApplicationId, Token, message, options);

    public Task<RestMessage> GetFollowupMessageAsync(Snowflake messageId, RequestProperties? options = null) => _client.GetInteractionFollowupMessageAsync(ApplicationId, Token, messageId, options);

    public Task<RestMessage> ModifyFollowupMessageAsync(Snowflake messageId, Action<MessageOptions> action, RequestProperties? options = null) => _client.ModifyInteractionFollowupMessageAsync(ApplicationId, Token, messageId, action, options);

    public Task DeleteFollowupMessageAsync(Snowflake messageId, RequestProperties? options = null) => _client.DeleteInteractionFollowupMessageAsync(ApplicationId, Token, messageId, options);
}