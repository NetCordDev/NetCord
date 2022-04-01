using System.Globalization;

using NetCord.JsonModels;

namespace NetCord;

public abstract class Interaction : ClientEntity
{
    private readonly JsonInteraction _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public Snowflake ApplicationId => _jsonEntity.ApplicationId;

    public InteractionType Type => _jsonEntity.Type;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public Guild? Guild { get; }

    public Snowflake? ChannelId => _jsonEntity.ChannelId;

    public TextChannel? Channel { get; }

    public User User { get; }

    public string Token => _jsonEntity.Token;

    public CultureInfo UserLocale => _jsonEntity.UserLocale;

    public CultureInfo? GuildLocale => _jsonEntity.GuildLocale;

    public abstract InteractionData Data { get; }

    internal Interaction(JsonInteraction jsonEntity, GatewayClient client) : base(client.Rest)
    {
        _jsonEntity = jsonEntity;
        var guildId = jsonEntity.GuildId;
        if (guildId.HasValue && client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out Guild? guild))
        {
            Guild = guild;
            User = new GuildInteractionUser(jsonEntity.GuildUser!, Guild, client.Rest);
            if (ChannelId.HasValue)
            {
                if (guild.Channels.TryGetValue(ChannelId.GetValueOrDefault(), out var channel))
                    Channel = (TextChannel)channel;
                else if (guild.ActiveThreads.TryGetValue(ChannelId.GetValueOrDefault(), out var thread))
                    Channel = thread;
            }
        }
        else
        {
            User = new User(jsonEntity.User!, client.Rest);
            if (ChannelId.HasValue)
            {
                if (client.DMChannels.TryGetValue(ChannelId.GetValueOrDefault(), out var dMChannel))
                    Channel = dMChannel;
                else if (client.GroupDMChannels.TryGetValue(ChannelId.GetValueOrDefault(), out var groupDMChannel))
                    Channel = groupDMChannel;
            }
        }
    }

    internal static Interaction CreateFromJson(JsonInteraction jsonEntity, GatewayClient client)
    {
        return jsonEntity.Type switch
        {
            InteractionType.ApplicationCommand => jsonEntity.Data.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonEntity, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonEntity, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonEntity, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonEntity.Data.ComponentType switch
            {
                ComponentType.Button => new ButtonInteraction(jsonEntity, client),
                ComponentType.Menu => new MenuInteraction(jsonEntity, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.ApplicationCommandAutocomplete => new ApplicationCommandAutocompleteInteraction(jsonEntity, client),
            InteractionType.ModalSubmit => new ModalSubmitInteraction(jsonEntity, client),
            _ => throw new InvalidOperationException(),
        };
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