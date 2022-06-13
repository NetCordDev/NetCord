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

    public Interaction(JsonInteraction jsonModel, GatewayClient client) : base(client.Rest)
    {
        _jsonModel = jsonModel;
        var guildId = jsonModel.GuildId;
        if (guildId.HasValue && client.Guilds.TryGetValue(guildId.GetValueOrDefault(), out Guild? guild))
        {
            Guild = guild;
            User = new GuildInteractionUser(jsonModel.GuildUser!, Guild, client.Rest);
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
            User = new User(jsonModel.User!, client.Rest);
            if (ChannelId.HasValue)
            {
                if (client.DMChannels.TryGetValue(ChannelId.GetValueOrDefault(), out var dMChannel))
                    Channel = dMChannel;
                else if (client.GroupDMChannels.TryGetValue(ChannelId.GetValueOrDefault(), out var groupDMChannel))
                    Channel = groupDMChannel;
            }
        }
    }

    internal static Interaction CreateFromJson(JsonInteraction jsonModel, GatewayClient client)
    {
        return jsonModel.Type switch
        {
            InteractionType.ApplicationCommand => jsonModel.Data.Type switch
            {
                ApplicationCommandType.ChatInput => new SlashCommandInteraction(jsonModel, client),
                ApplicationCommandType.User => new UserCommandInteraction(jsonModel, client),
                ApplicationCommandType.Message => new MessageCommandInteraction(jsonModel, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.MessageComponent => jsonModel.Data.ComponentType switch
            {
                ComponentType.Button => new ButtonInteraction(jsonModel, client),
                ComponentType.Menu => new MenuInteraction(jsonModel, client),
                _ => throw new InvalidOperationException(),
            },
            InteractionType.ApplicationCommandAutocomplete => new ApplicationCommandAutocompleteInteraction(jsonModel, client),
            InteractionType.ModalSubmit => new ModalSubmitInteraction(jsonModel, client),
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