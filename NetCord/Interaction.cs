using System.Globalization;

using NetCord.JsonModels;

namespace NetCord;

public abstract class Interaction : ClientEntity
{
    internal JsonInteraction _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public DiscordId ApplicationId => _jsonEntity.ApplicationId;

    public InteractionType Type => _jsonEntity.Type;

    //public DiscordId? GuildId => _jsonEntity.GuildId;
    public Guild? Guild { get; }

    public TextChannel? Channel { get; }

    public DiscordId? ChannelId => _jsonEntity.ChannelId;

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
            User = new GuildUser(jsonEntity.GuildUser!, Guild, client.Rest);
            if (ChannelId.HasValue)
            {
                if (guild._channels.TryGetValue(ChannelId.GetValueOrDefault(), out var channel))
                    Channel = (TextChannel)channel;
                else if (guild._activeThreads.TryGetValue(ChannelId.GetValueOrDefault(), out var thread))
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
        switch (jsonEntity.Type)
        {
            case InteractionType.ApplicationCommand:
                return new ApplicationCommandInteraction(jsonEntity, client);
            case InteractionType.MessageComponent:
                var componentType = jsonEntity.Data.ComponentType;
                if (componentType == ComponentType.Button)
                    return new ButtonInteraction(jsonEntity, client);
                else
                    return new MenuInteraction(jsonEntity, client);
            case InteractionType.ApplicationCommandAutocomplete:
                return new ApplicationCommandAutocompleteInteraction(jsonEntity, client);
            case InteractionType.ModalSubmit:
                return new ModalSubmitInteraction(jsonEntity, client);
        }
        throw new InvalidOperationException();
    }

    public Task SendResponseAsync(InteractionCallback interactionCallback, RequestOptions? options = null) => _client.Interaction.SendResponseAsync(Id, Token, interactionCallback, options);

    public Task<RestMessage> GetResponseAsync(RequestOptions? options = null) => _client.Interaction.GetResponseAsync(ApplicationId, Token, options);

    public Task<RestMessage> ModifyResponseAsync(Action<MessageOptions> action, RequestOptions? options = null) => _client.Interaction.ModifyResponseAsync(ApplicationId, Token, action, options);

    public Task DeleteResponseAsync(RequestOptions? options = null) => _client.Interaction.DeleteResponseAsync(ApplicationId, Token, options);

    public Task<RestMessage> SendFollowupMessageAsync(InteractionMessageProperties message, RequestOptions? options = null) => _client.Interaction.SendFollowupMessageAsync(ApplicationId, Token, message, options);

    public Task<RestMessage> GetFollowupMessageAsync(DiscordId messageId, RequestOptions? options = null) => _client.Interaction.GetFollowupMessageAsync(ApplicationId, Token, messageId, options);

    public Task<RestMessage> ModifyFollowupMessageAsync(DiscordId messageId, Action<MessageOptions> action, RequestOptions? options = null) => _client.Interaction.ModifyFollowupMessageAsync(ApplicationId, Token, messageId, action, options);

    public Task DeleteFollowupMessageAsync(DiscordId messageId, RequestOptions? options = null) => _client.Interaction.DeleteFollowupMessageAsync(ApplicationId, Token, messageId, options);
}