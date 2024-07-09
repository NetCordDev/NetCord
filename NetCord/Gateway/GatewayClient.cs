using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.Gateway.Compression;
using NetCord.Gateway.JsonModels;

using WebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;

namespace NetCord.Gateway;

/// <summary>
/// The WebSocket-based client allows you to receive events from the Discord Gateway and update or request resources, such as updating voice state.
/// </summary>
public partial class GatewayClient : WebSocketClient, IEntity
{
    private readonly GatewayClientConfiguration _configuration;
    private readonly object? _DMsLock;
    private readonly Dictionary<ulong, SemaphoreSlim>? _DMSemaphores;
    private readonly IGatewayCompression _compression;
    private readonly bool _disposeRest;

    #region Events

    /// <summary>
    /// The ready event is dispatched when a client has completed the initial handshake with the Gateway (for new sessions).
    /// The ready event contains all the state required for a client to begin interacting with the rest of the platform.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<ReadyEventArgs, ValueTask>? Ready;

    /// <summary>
    /// Sent when an application command's permissions are updated.
    /// The inner payload is an <see cref="ApplicationCommandPermission"/> object.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<ApplicationCommandPermission, ValueTask>? ApplicationCommandPermissionsUpdate;

    /// <summary>
    /// Sent when a rule is created.
    /// The inner payload is an <see cref="AutoModerationRule"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.AutoModerationConfiguration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleCreate;

    /// <summary>
    /// Sent when a rule is updated.
    /// The inner payload is an <see cref="AutoModerationRule"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.AutoModerationConfiguration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleUpdate;

    /// <summary>
    /// Sent when a rule is deleted.
    /// The inner payload is an <see cref="AutoModerationRule"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.AutoModerationConfiguration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<AutoModerationRule, ValueTask>? AutoModerationRuleDelete;

    /// <summary>
    /// Sent when a rule is triggered and an action is executed (e.g. when a message is blocked).<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.AutoModerationExecution"/>
    /// <br/> Optional Intents:
    /// <list type="bullet">
    ///		<item>
    ///			<term>
    ///			<see cref="GatewayIntents.MessageContent"/>
    ///			</term>
    ///			<description>
    ///			For receiving <see cref="AutoModerationActionExecutionEventArgs.Content"/> and <see cref="AutoModerationActionExecutionEventArgs.MatchedContent"/>.
    ///			</description>
    ///		</item>
    /// </list>
    /// </summary>
    public event Func<AutoModerationActionExecutionEventArgs, ValueTask>? AutoModerationActionExecution;

    /// <summary>
    /// Sent when a new guild channel is created, relevant to the bot.
    /// The inner payload is an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<IGuildChannel, ValueTask>? GuildChannelCreate;

    /// <summary>
    /// Sent when a channel is updated. This is not sent with new messages, those are tracked by <see cref="MessageCreate"/> and <see cref="GuildThreadCreate"/>. This event may reference roles or guild members that no longer exist in the guild.
    /// The inner payload is an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<IGuildChannel, ValueTask>? GuildChannelUpdate;

    /// <summary>
    /// Sent when a channel relevant to the bot is deleted.
    /// The inner payload is an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<IGuildChannel, ValueTask>? GuildChannelDelete;

    /// <summary>
    /// Sent when a thread is created, relevant to the bot, or when the current user is added to a thread.
    /// The inner payload is an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildThreadCreateEventArgs, ValueTask>? GuildThreadCreate;

    /// <summary>
    /// Sent when a thread is updated. This is not sent with new messages, those are tracked by <see cref="MessageCreate"/>.
    /// The inner payload is an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildThread, ValueTask>? GuildThreadUpdate;

    /// <summary>
    /// Sent when a thread relevant to the bot is deleted.
    /// The inner payload is a subset of an <see cref="IGuildChannel"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildThreadDeleteEventArgs, ValueTask>? GuildThreadDelete;

    /// <summary>
    /// Sent when the current user gains access to a channel.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildThreadListSyncEventArgs, ValueTask>? GuildThreadListSync;

    /// <summary>
    /// Sent when the <see cref="Rest.GuildThreadUser"/> object for the bot is updated. This event is largely just a signal that you are a member of the thread.
    /// The inner payload is a <see cref="GuildThreadUserUpdateEventArgs"/> object with a set <see cref="GuildThreadUsersUpdateEventArgs.GuildId"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildThreadUserUpdateEventArgs, ValueTask>? GuildThreadUserUpdate;

    /// <summary>
    /// Sent when anyone is added to or removed from a thread.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>, <see cref="GatewayIntents.GuildUsers"/>*
    /// <br/> Optional Intents:
    /// <list type="bullet">
    ///		<item>
    ///			<term>
    ///			<see cref="GatewayIntents.GuildUsers"/>
    ///			</term>
    ///			<description>
    ///			For receiving this event when other users are added / removed, otherwise this event will only fire for the bot's user.
    ///			</description>
    ///		</item>
    /// </list>
    /// <br/><br/>
    /// *Must also be enabled in the developer portal.
    /// </summary>
    public event Func<GuildThreadUsersUpdateEventArgs, ValueTask>? GuildThreadUsersUpdate;

    /// <summary>
    /// Sent when a message is pinned or unpinned in a text channel. This is not sent when a pinned message is deleted.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>, <see cref="GatewayIntents.DirectMessages"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<ChannelPinsUpdateEventArgs, ValueTask>? ChannelPinsUpdate;

    /// <summary>
    /// Sent when an entitlement is created.
    /// The inner payload is an <see cref="Entitlement"/> object.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Entitlement, ValueTask>? EntitlementCreate;

    /// <summary>
    /// Sent when an entitlement is updated. When an entitlement for a subscription is renewed, the <see cref="Entitlement.EndsAt"/> field may have an updated value with the new expiration date.
    /// The inner payload is an <see cref="Entitlement"/> object.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Entitlement, ValueTask>? EntitlementUpdate;

    /// <summary>
    /// Sent when an entitlement is deleted. Entitlements are not deleted when they expire.
    /// The inner payload is an <see cref="Entitlement"/> object.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Entitlement, ValueTask>? EntitlementDelete;

    /// <summary>
    /// This event can be sent in three different scenarios (During an outage, the <see cref="Guild"/> object in scenarios 1 and 3 may be marked as unavailable):<br/>
    /// <list type="bullet">
    ///		<item>
    ///			<description>
    ///			To lazily load and backfill information for all unavailable guilds sent in the <see cref="Ready"/> event. Guilds unavailable due to an outage will send a <see cref="GuildDelete"/> event.
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///			When a guild becomes available again to the client.
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///			When the current user joins a new guild.
    ///			</description>
    ///		</item>
    /// </list>
    /// The inner payload can be a <see cref="Guild"/> object with extra fields, or an unavailable <see cref="Guild"/> object. If the guild has over 75k users, users and presences returned in this event will only contain your bot and users in voice channels.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents:
    /// <list type="bullet">
    ///		<item>
    ///			<term>
    ///			<see cref="GatewayIntents.GuildPresences"/>
    ///			</term>
    ///			<description>
    ///			For receiving users and presences other than the bot's user and users in voice channels (Same as the 75k limit).
    ///			</description>
    ///		</item>
    /// </list>
    /// </summary>
    public event Func<GuildCreateEventArgs, ValueTask>? GuildCreate;

    /// <summary>
    /// Sent when a guild is updated.
    /// The inner payload is a <see cref="Guild"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Guild, ValueTask>? GuildUpdate;

    /// <summary>
    /// Sent when a guild becomes or was already unavailable due to an outage, or when the bot leaves / is removed from a guild.
    /// The inner payload is an unavailable guild object. If the <see cref="GuildDeleteEventArgs.IsUserDeleted"/> field is not true, the bot was removed from the guild.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildDeleteEventArgs, ValueTask>? GuildDelete;

    /// <summary>
    /// Sent when a guild audit log entry is created.
    /// The inner payload is an <see cref="AuditLogEntry"/> object. This event is only sent to bots with the <see cref="Permissions.ViewAuditLog"/> permission.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildModeration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<AuditLogEntry, ValueTask>? GuildAuditLogEntryCreate;

    /// <summary>
    /// Sent when a user is banned from a guild.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildModeration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanAdd;

    /// <summary>
    /// Sent when a user is unbanned from a guild.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildModeration"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildBanEventArgs, ValueTask>? GuildBanRemove;

    /// <summary>
    /// Sent when a guild's emojis have been updated.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildEmojisAndStickers"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildEmojisUpdateEventArgs, ValueTask>? GuildEmojisUpdate;

    /// <summary>
    /// Sent when a guild's stickers have been updated.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildEmojisAndStickers"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildStickersUpdateEventArgs, ValueTask>? GuildStickersUpdate;

    /// <summary>
    /// Sent when guild integrations are updated.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildIntegrations"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildIntegrationsUpdateEventArgs, ValueTask>? GuildIntegrationsUpdate;

    /// <summary>
    /// Sent when a new user joins a guild.
    /// The inner payload is a <see cref="GuildUser"/> object with an extra <c>guild_id</c> key.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildUsers"/>*
    /// <br/> Optional Intents: None
    /// <br/><br/>
    /// *Must also be enabled in the developer portal.
    /// </summary>
    public event Func<GuildUser, ValueTask>? GuildUserAdd;

    /// <summary>
    /// Sent when a user is removed from a guild (leave/kick/ban).<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildUsers"/>*
    /// <br/> Optional Intents: None
    /// <br/><br/>
    /// *Must also be enabled in the developer portal.
    /// </summary>
    public event Func<GuildUserRemoveEventArgs, ValueTask>? GuildUserRemove;

    /// <summary>
    /// Sent when a guild user is updated. This will also fire when the <see cref="GuildUser"/> object of a guild user changes.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildUsers"/>*
    /// <br/> Optional Intents: None
    /// <br/><br/>
    /// *Must also be enabled in the developer portal.
    /// </summary>
    public event Func<GuildUser, ValueTask>? GuildUserUpdate;

    /// <summary>
    /// Sent in response to <see cref="RequestGuildUsersAsync(GuildUsersRequestProperties, CancellationToken)"/>. You can use the <see cref="GuildUserChunkEventArgs.ChunkIndex"/> and <see cref="GuildUserChunkEventArgs.ChunkCount"/> to calculate how many chunks are left for your request.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildUserChunkEventArgs, ValueTask>? GuildUserChunk;

    /// <summary>
    /// Sent when a guild role is created.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Role, ValueTask>? RoleCreate;

    /// <summary>
    /// Sent when a guild role is updated.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Role, ValueTask>? RoleUpdate;

    /// <summary>
    /// Sent when a guild role is deleted.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<RoleDeleteEventArgs, ValueTask>? RoleDelete;

    /// <summary>
    /// Sent when a guild scheduled event is created.
    /// The inner payload is a <see cref="GuildScheduledEvent"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildScheduledEvents"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventCreate;

    /// <summary>
    /// Sent when a guild scheduled event is updated.
    /// The inner payload is a <see cref="GuildScheduledEvent"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildScheduledEvents"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventUpdate;

    /// <summary>
    /// Sent when a guild scheduled event is deleted.
    /// The inner payload is a <see cref="GuildScheduledEvent"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildScheduledEvents"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildScheduledEvent, ValueTask>? GuildScheduledEventDelete;

    /// <summary>
    /// Sent when a user has subscribed to a guild scheduled event.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildScheduledEvents"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserAdd;

    /// <summary>
    /// Sent when a user has unsubscribed from a guild scheduled event.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildScheduledEvents"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildScheduledEventUserEventArgs, ValueTask>? GuildScheduledEventUserRemove;

    /// <summary>
    /// Sent when an integration is created.
    /// The inner payload is an integration object with a set <see cref="GuildIntegrationEventArgs.GuildId"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildIntegrations"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildIntegrationEventArgs, ValueTask>? GuildIntegrationCreate;

    /// <summary>
    /// Sent when an integration is updated.
    /// The inner payload is an integration object with a set <see cref="GuildIntegrationEventArgs.GuildId"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildIntegrations"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildIntegrationEventArgs, ValueTask>? GuildIntegrationUpdate;

    /// <summary>
    /// Sent when an integration is deleted.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildIntegrations"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildIntegrationDeleteEventArgs, ValueTask>? GuildIntegrationDelete;

    /// <summary>
    /// Sent when a new invite to a channel is created. Only sent if the bot has the <see cref="Permissions.ManageChannels"/> permission for the relevant channel.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildInvites"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildInvite, ValueTask>? GuildInviteCreate;

    /// <summary>
    /// Sent when an invite is deleted. Only sent if the bot has the <see cref="Permissions.ManageChannels"/> permission for the relevant channel.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildInvites"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<GuildInviteDeleteEventArgs, ValueTask>? GuildInviteDelete;

    /// <summary>
    /// Sent when a message is created.
    /// The inner payload is a message object with set <see cref="Message.GuildId"/>, and <see cref="Rest.RestMessage.Author"/> fields.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessages"/>, <see cref="GatewayIntents.DirectMessages"/>*
    /// <br/> Optional Intents:
    /// <list type="bullet">
    ///		<item>
    ///			<term>
    ///			<see cref="GatewayIntents.MessageContent"/>
    ///			</term>
    ///			<description>
    ///			For receiving <see cref="Rest.RestMessage.Content"/>, <see cref="Rest.RestMessage.Embeds"/>, <see cref="Rest.RestMessage.Attachments"/> and <see cref="Rest.RestMessage.Embeds"/>.<br/>
    ///			This does not apply to:
    ///				<list type="bullet">
    ///					<item>
    ///						<description>
    ///						Content in messages sent by the bot.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content in DMs with the bot.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content in which the bot is mentioned.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content of messages a message context menu command is used on.
    ///						</description>
    ///					</item>
    ///				</list>
    ///			</description>
    ///		</item>
    /// </list>
    /// <br/><br/>
    /// *Ephemeral messages do not use the guild channel. Because of this, they are tied to the <see cref="GatewayIntents.DirectMessages"/> intent, and the message object won't include a <see cref="Message.GuildId"/> or <see cref="Rest.RestMessage.Author"/>.
    /// </summary>
    public event Func<Message, ValueTask>? MessageCreate;

    /// <summary>
    /// Sent when a message is updated.
    /// The inner payload is a partial message object, with only the message's ID and Guild ID being guaranteed present, all other fields can be null.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessages"/>, <see cref="GatewayIntents.DirectMessages"/>*
    /// <br/> Optional Intents:
    /// <list type="bullet">
    ///		<item>
    ///			<term>
    ///			<see cref="GatewayIntents.MessageContent"/>
    ///			</term>
    ///			<description>
    ///			For receiving <see cref="Rest.RestMessage.Content"/>, <see cref="Rest.RestMessage.Embeds"/>, <see cref="Rest.RestMessage.Attachments"/> and <see cref="Rest.RestMessage.Embeds"/>.<br/>
    ///			This does not apply to:
    ///				<list type="bullet">
    ///					<item>
    ///						<description>
    ///						Content in messages sent by the bot.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content in DMs with the bot.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content in which the bot is mentioned.
    ///						</description>
    ///					</item>
    ///					<item>
    ///						<description>
    ///						Content of messages a message context menu command is used on.
    ///						</description>
    ///					</item>
    ///				</list>
    ///			</description>
    ///		</item>
    /// </list>
    /// <br/><br/>
    /// *Ephemeral messages do not use the guild channel. Because of this, they are tied to the <see cref="GatewayIntents.DirectMessages"/> intent, and the message object won't include a <see cref="Message.GuildId"/> or <see cref="Rest.RestMessage.Author"/>.
    /// </summary>
    public event Func<IPartialMessage, ValueTask>? MessageUpdate;

    /// <summary>
    /// Sent when a message is deleted.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessages"/>, <see cref="GatewayIntents.DirectMessages"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageDeleteEventArgs, ValueTask>? MessageDelete;

    /// <summary>
    /// Sent when multiple messages are deleted at once.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessages"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageDeleteBulkEventArgs, ValueTask>? MessageDeleteBulk;

    /// <summary>
    /// Sent when a user adds a reaction to a message.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessageReactions"/>, <see cref="GatewayIntents.DirectMessageReactions"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageReactionAddEventArgs, ValueTask>? MessageReactionAdd;

    /// <summary>
    /// Sent when a user removes a reaction from a message.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessageReactions"/>, <see cref="GatewayIntents.DirectMessageReactions"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageReactionRemoveEventArgs, ValueTask>? MessageReactionRemove;

    /// <summary>
    /// Sent when a user explicitly removes all reactions from a message.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessageReactions"/>, <see cref="GatewayIntents.DirectMessageReactions"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageReactionRemoveAllEventArgs, ValueTask>? MessageReactionRemoveAll;

    /// <summary>
    /// Sent when a user removes all instances of a given emoji from the reactions of a message.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessageReactions"/>, <see cref="GatewayIntents.DirectMessageReactions"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<MessageReactionRemoveEmojiEventArgs, ValueTask>? MessageReactionRemoveEmoji;

    /// <summary>
    /// Sent when a user's presence or info, such as their name or avatar, is updated. Requires the <see cref="GatewayIntents.GuildPresences"/> intent.
    /// The user object within this event can be partial, with the id being the only required field, everything else is optional. Along with this limitation, no fields are required, and the types of the fields are <b>not validated</b>. You should expect <b>any</b> combination of fields and types within this event.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildPresences"/>*
    /// <br/> Optional Intents: None
    /// <br/><br/>
    /// *Must also be enabled in the developer portal.
    /// </summary>
    public event Func<Presence, ValueTask>? PresenceUpdate;

    /// <summary>
    /// Sent when a user starts typing in a channel, and fires again every 10 seconds while they continue typing.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildMessageTyping"/>, <see cref="GatewayIntents.DirectMessageTyping"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<TypingStartEventArgs, ValueTask>? TypingStart;

    /// <summary>
    /// Sent when properties about the current bot's user change.
    /// Inner payload is a <see cref="CurrentUser"/> object.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<CurrentUser, ValueTask>? CurrentUserUpdate;

    /// <summary>
    /// Sent when someone joins/leaves/moves voice channels.
    /// Inner payload is a <see cref="VoiceState"/> object.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildVoiceStates"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<VoiceState, ValueTask>? VoiceStateUpdate;

    /// <summary>
    /// Sent when a guild's voice server is updated. This is sent when initially connecting to voice, and when the current voice instance fails over to a new server.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<VoiceServerUpdateEventArgs, ValueTask>? VoiceServerUpdate;

    /// <summary>
    /// Sent when a guild channel's webhook is created, updated, or deleted.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.GuildWebhooks"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<WebhooksUpdateEventArgs, ValueTask>? WebhooksUpdate;

    public event Func<MessagePollVoteEventArgs, ValueTask>? MessagePollVoteAdd;

    public event Func<MessagePollVoteEventArgs, ValueTask>? MessagePollVoteRemove;

    /// <summary>
    /// Sent when a user uses an interaction.
    /// Inner payload is an <see cref="Interaction"/>.<br/>
    /// <br/> Required Intents: None
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<Interaction, ValueTask>? InteractionCreate;

    /// <summary>
    /// Sent when a <see cref="StageInstance"/> is created (i.e. the Stage is now 'live').
    /// Inner payload is a <see cref="StageInstance"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<StageInstance, ValueTask>? StageInstanceCreate;

    /// <summary>
    /// Sent when a <see cref="StageInstance"/> is updated.
    /// Inner payload is a <see cref="StageInstance"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<StageInstance, ValueTask>? StageInstanceUpdate;

    /// <summary>
    /// Sent when a <see cref="StageInstance"/> is deleted (i.e. the Stage has been closed).
    /// Inner payload is a <see cref="StageInstance"/>.<br/>
    /// <br/> Required Intents: <see cref="GatewayIntents.Guilds"/>
    /// <br/> Optional Intents: None
    /// </summary>
    public event Func<StageInstance, ValueTask>? StageInstanceDelete;

    /// <summary>
    /// Not documented by Discord.
    /// </summary>
    public event Func<GuildJoinRequestUpdateEventArgs, ValueTask>? GuildJoinRequestUpdate;

    /// <summary>
    /// Not documented by Discord.
    /// </summary>
    public event Func<GuildJoinRequestDeleteEventArgs, ValueTask>? GuildJoinRequestDelete;

    /// <summary>
    /// An unknown event.
    /// </summary>
    public event Func<UnknownEventEventArgs, ValueTask>? UnknownEvent;

    #endregion

    /// <summary>
    /// The token of the <see cref="GatewayClient"/>.
    /// </summary>
    public IEntityToken Token { get; }

    /// <summary>
    /// The cache of the <see cref="GatewayClient"/>.
    /// </summary>
    public IGatewayClientCache Cache { get; private set; }

    /// <summary>
    /// The session id of the <see cref="GatewayClient"/>.
    /// </summary>
    public string? SessionId { get; private set; }

    /// <summary>
    /// The sequence number of the <see cref="GatewayClient"/>.
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// The shard of the <see cref="GatewayClient"/>.
    /// </summary>
    public Shard? Shard => _configuration.Shard;

    /// <summary>
    /// The application flags of the <see cref="GatewayClient"/>.
    /// </summary>
    public ApplicationFlags ApplicationFlags { get; private set; }

    /// <summary>
    /// The <see cref="Rest.RestClient"/> of the <see cref="GatewayClient"/>.
    /// </summary>
    public Rest.RestClient Rest { get; }

    public ulong Id => Token.Id;

    private protected override Uri Uri { get; }

    public DateTimeOffset CreatedAt => Token.CreatedAt;

    /// <summary>
    /// Constructs a <see cref="GatewayClient"/> using the given <paramref name="token"/> and <paramref name="configuration"/>.
    /// </summary>
    public GatewayClient(IEntityToken token, GatewayClientConfiguration? configuration = null) : this(token, new(token, (configuration ??= new()).RestClientConfiguration), configuration)
    {
        _disposeRest = true;
    }

    internal GatewayClient(IEntityToken token, Rest.RestClient rest, GatewayClientConfiguration configuration) : base(configuration ??= new())
    {
        Token = token;

        _configuration = configuration;

        var compression = _compression = configuration.Compression ?? IGatewayCompression.CreateDefault();
        Uri = new($"wss://{configuration.Hostname ?? Discord.GatewayHostname}/?v={(int)configuration.Version}&encoding=json&compress={compression.Name}", UriKind.Absolute);
        Cache = configuration.Cache ?? new GatewayClientCache();
        Rest = rest;

        if (configuration.CacheDMChannels)
        {
            _DMsLock = new();
            _DMSemaphores = [];
        }
    }

    private protected override void OnConnected()
    {
        _compression.Initialize();
    }

    private ValueTask SendIdentifyAsync(PresenceProperties? presence = null, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new GatewayPayloadProperties<GatewayIdentifyProperties>(GatewayOpcode.Identify, new(Token.RawToken)
        {
            ConnectionProperties = _configuration.ConnectionProperties ?? ConnectionPropertiesProperties.Default,
            LargeThreshold = _configuration.LargeThreshold,
            Shard = _configuration.Shard,
            Presence = presence ?? _configuration.Presence,
            Intents = _configuration.Intents,
        }).Serialize(Serialization.Default.GatewayPayloadPropertiesGatewayIdentifyProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    /// <summary>
    /// Starts the <see cref="GatewayClient"/>.
    /// </summary>
    /// <param name="presence">The presence to set.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns></returns>
    public async Task StartAsync(PresenceProperties? presence = null, CancellationToken cancellationToken = default)
    {
        await StartAsync(cancellationToken).ConfigureAwait(false);
        await SendIdentifyAsync(presence, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resumes the session specified by <paramref name="sessionId"/>.
    /// </summary>
    /// <param name="sessionId">The session to resume.</param>
    /// <param name="sequenceNumber">The sequence number of the payload to resume from.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns></returns>
    public async Task ResumeAsync(string sessionId, int sequenceNumber, CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken).ConfigureAwait(false);
        await TryResumeAsync(SessionId = sessionId, SequenceNumber = sequenceNumber, cancellationToken).ConfigureAwait(false);
    }

    private protected override bool Reconnect(WebSocketCloseStatus? status, string? description)
        => status is not ((WebSocketCloseStatus)4004 or (WebSocketCloseStatus)4010 or (WebSocketCloseStatus)4011 or (WebSocketCloseStatus)4012 or (WebSocketCloseStatus)4013 or (WebSocketCloseStatus)4014);

    private protected override ValueTask TryResumeAsync(CancellationToken cancellationToken = default)
    {
        return TryResumeAsync(SessionId!, SequenceNumber, cancellationToken);
    }

    private ValueTask TryResumeAsync(string sessionId, int sequenceNumber, CancellationToken cancellationToken = default)
    {
        var serializedPayload = new GatewayPayloadProperties<GatewayResumeProperties>(GatewayOpcode.Resume, new(Token.RawToken, sessionId, sequenceNumber)).Serialize(Serialization.Default.GatewayPayloadPropertiesGatewayResumeProperties);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    private protected override ValueTask HeartbeatAsync(CancellationToken cancellationToken = default)
    {
        var serializedPayload = new GatewayPayloadProperties<int>(GatewayOpcode.Heartbeat, SequenceNumber).Serialize(Serialization.Default.GatewayPayloadPropertiesInt32);
        _latencyTimer.Start();
        return SendPayloadAsync(serializedPayload, cancellationToken);
    }

    private protected override JsonPayload CreatePayload(ReadOnlyMemory<byte> payload) => JsonSerializer.Deserialize(_compression.Decompress(payload).Span, Serialization.Default.JsonPayload)!;

    private protected override async Task ProcessPayloadAsync(JsonPayload payload)
    {
        switch ((GatewayOpcode)payload.Opcode)
        {
            case GatewayOpcode.Dispatch:
                SequenceNumber = payload.SequenceNumber.GetValueOrDefault();
                try
                {
                    await ProcessEventAsync(payload).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                break;
            case GatewayOpcode.Heartbeat:
                break;
            case GatewayOpcode.Reconnect:
                InvokeLog(LogMessage.Info("Reconnect request"));
                await AbortAndReconnectAsync().ConfigureAwait(false);
                break;
            case GatewayOpcode.InvalidSession:
                InvokeLog(LogMessage.Info("Invalid session"));
                try
                {
                    await SendIdentifyAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    InvokeLog(LogMessage.Error(ex));
                }
                break;
            case GatewayOpcode.Hello:
                StartHeartbeating(payload.Data.GetValueOrDefault().ToObject(Serialization.Default.JsonHello).HeartbeatInterval);
                break;
            case GatewayOpcode.HeartbeatACK:
                await UpdateLatencyAsync(_latencyTimer.Elapsed).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Joins, moves, or disconnects the app from a voice channel.
    /// </summary>
    public ValueTask UpdateVoiceStateAsync(VoiceStateProperties voiceState, CancellationToken cancellationToken = default)
    {
        GatewayPayloadProperties<VoiceStateProperties> payload = new(GatewayOpcode.VoiceStateUpdate, voiceState);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesVoiceStateProperties), cancellationToken);
    }

    /// <summary>
    /// Updates an app's presence.
    /// </summary>
    /// <param name="presence">The presence to set.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    public ValueTask UpdatePresenceAsync(PresenceProperties presence, CancellationToken cancellationToken = default)
    {
        GatewayPayloadProperties<PresenceProperties> payload = new(GatewayOpcode.PresenceUpdate, presence);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesPresenceProperties), cancellationToken);
    }

    /// <summary>
    /// Requests user for a guild.
    /// </summary>
    public ValueTask RequestGuildUsersAsync(GuildUsersRequestProperties requestProperties, CancellationToken cancellationToken = default)
    {
        GatewayPayloadProperties<GuildUsersRequestProperties> payload = new(GatewayOpcode.RequestGuildUsers, requestProperties);
        return SendPayloadAsync(payload.Serialize(Serialization.Default.GatewayPayloadPropertiesGuildUsersRequestProperties), cancellationToken);
    }

    private async Task ProcessEventAsync(JsonPayload payload)
    {
        var data = payload.Data.GetValueOrDefault();
        var name = payload.Event!;
        switch (name)
        {
            case "READY":
                {
                    var latency = _latencyTimer.Elapsed;
                    InvokeLog(LogMessage.Info("Ready"));
                    var updateLatencyTask = UpdateLatencyAsync(latency);
                    ReadyEventArgs args = new(data.ToObject(Serialization.Default.JsonReadyEventArgs), Rest);
                    await InvokeEventAsync(Ready, args, data =>
                    {
                        var cache = Cache;
                        cache = cache.CacheCurrentUser(data.User);
                        cache = cache.SyncGuilds(data.GuildIds);
                        Cache = cache;

                        SessionId = args.SessionId;
                        ApplicationFlags = args.ApplicationFlags;

                        _readyCompletionSource.TrySetResult();
                    }).ConfigureAwait(false);
                    await updateLatencyTask.ConfigureAwait(false);
                }
                break;
            case "RESUMED":
                {
                    var latency = _latencyTimer.Elapsed;
                    InvokeLog(LogMessage.Info("Resumed"));
                    var updateLatencyTask = UpdateLatencyAsync(latency);
                    var resumeTask = InvokeResumeEventAsync();

                    _readyCompletionSource.TrySetResult();

                    await updateLatencyTask.ConfigureAwait(false);
                    await resumeTask.ConfigureAwait(false);
                }
                break;
            case "APPLICATION_COMMAND_PERMISSIONS_UPDATE":
                {
                    await InvokeEventAsync(ApplicationCommandPermissionsUpdate, () => new(data.ToObject(Serialization.Default.JsonApplicationCommandGuildPermission))).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_CREATE":
                {
                    await InvokeEventAsync(AutoModerationRuleCreate, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_UPDATE":
                {
                    await InvokeEventAsync(AutoModerationRuleUpdate, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_RULE_DELETE":
                {
                    await InvokeEventAsync(AutoModerationRuleDelete, () => new(data.ToObject(Serialization.Default.JsonAutoModerationRule), Rest)).ConfigureAwait(false);
                }
                break;
            case "AUTO_MODERATION_ACTION_EXECUTION":
                {
                    await InvokeEventAsync(AutoModerationActionExecution, () => new(data.ToObject(Serialization.Default.JsonAutoModerationActionExecutionEventArgs))).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = IGuildChannel.CreateFromJson(json, json.GuildId.GetValueOrDefault(), Rest);
                    await InvokeEventAsync(GuildChannelCreate, channel, channel => Cache = Cache.CacheGuildChannel(channel)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = IGuildChannel.CreateFromJson(json, json.GuildId.GetValueOrDefault(), Rest);
                    await InvokeEventAsync(GuildChannelUpdate, channel, channel => Cache = Cache.CacheGuildChannel(channel)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var channel = IGuildChannel.CreateFromJson(json, json.GuildId.GetValueOrDefault(), Rest);
                    await InvokeEventAsync(GuildChannelDelete, channel, channel => Cache = Cache.RemoveGuildChannel(channel.GuildId, channel.Id)).ConfigureAwait(false);
                }
                break;
            case "CHANNEL_PINS_UPDATE":
                {
                    await InvokeEventAsync(ChannelPinsUpdate, () => new(data.ToObject(Serialization.Default.JsonChannelPinsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "THREAD_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var thread = GuildThread.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadCreate, () => new(thread, json.NewlyCreated.GetValueOrDefault()), () => Cache = Cache.CacheGuildThread(thread)).ConfigureAwait(false);
                }
                break;
            case "THREAD_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var thread = GuildThread.CreateFromJson(json, Rest);
                    await InvokeEventAsync(GuildThreadUpdate, thread, t => Cache = Cache.CacheGuildThread(t)).ConfigureAwait(false);
                }
                break;
            case "THREAD_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonChannel);
                    var guildId = json.GuildId.GetValueOrDefault();
                    await InvokeEventAsync(GuildThreadDelete, () => new(json.Id, guildId, json.ParentId.GetValueOrDefault(), json.Type), () => Cache = Cache.RemoveGuildThread(guildId, json.Id)).ConfigureAwait(false);
                }
                break;
            case "THREAD_LIST_SYNC":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildThreadListSyncEventArgs);
                    GuildThreadListSyncEventArgs args = new(json, Rest);
                    var guildId = args.GuildId;
                    await InvokeEventAsync(GuildThreadListSync, args, args => Cache = Cache.SyncGuildActiveThreads(guildId, args.Threads)).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBER_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUserUpdate, () => new(new(data.ToObject(Serialization.Default.JsonThreadUser), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "THREAD_MEMBERS_UPDATE":
                {
                    await InvokeEventAsync(GuildThreadUsersUpdate, () => new(data.ToObject(Serialization.Default.JsonGuildThreadUsersUpdateEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_CREATE":
                {
                    var jsonGuild = data.ToObject(Serialization.Default.JsonGuild);
                    var id = jsonGuild.Id;
                    if (jsonGuild.IsUnavailable)
                        await InvokeEventAsync(GuildCreate, () => new(id, null)).ConfigureAwait(false);
                    else
                    {
                        Guild guild = new(jsonGuild, Id, Rest);
                        await InvokeEventAsync(GuildCreate, () => new(id, guild), () => Cache = Cache.CacheGuild(guild)).ConfigureAwait(false);
                    }
                }
                break;
            case "GUILD_UPDATE":
                {
                    var guildId = GetGuildId();
                    if (Cache.Guilds.TryGetValue(guildId, out var oldGuild))
                        await InvokeEventAsync(GuildUpdate, new(data.ToObject(Serialization.Default.JsonGuild), Id, oldGuild), guild => Cache = Cache.CacheGuild(guild)).ConfigureAwait(false);
                }
                break;
            case "GUILD_DELETE":
                {
                    var jsonGuild = data.ToObject(Serialization.Default.JsonGuild);
                    await InvokeEventAsync(GuildDelete, () => new(jsonGuild.Id, !jsonGuild.IsUnavailable), () => Cache = Cache.RemoveGuild(jsonGuild.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                {
                    await InvokeEventAsync(GuildAuditLogEntryCreate, () => new(data.ToObject(Serialization.Default.JsonAuditLogEntry))).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_ADD":
                {
                    await InvokeEventAsync(GuildBanAdd, () => new(data.ToObject(Serialization.Default.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_BAN_REMOVE":
                {
                    await InvokeEventAsync(GuildBanRemove, () => new(data.ToObject(Serialization.Default.JsonGuildBanEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_EMOJIS_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildEmojisUpdateEventArgs);
                    await InvokeEventAsync(GuildEmojisUpdate, new(json, Rest), args => Cache = Cache.CacheGuildEmojis(args.GuildId, args.Emojis)).ConfigureAwait(false);
                }
                break;
            case "GUILD_STICKERS_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildStickersUpdateEventArgs);
                    await InvokeEventAsync(GuildStickersUpdate, new(json, Rest), args => Cache = Cache.CacheGuildStickers(args.GuildId, args.Stickers)).ConfigureAwait(false);
                }
                break;
            case "GUILD_INTEGRATIONS_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationsUpdate, () => new(data.ToObject(Serialization.Default.JsonGuildIntegrationsUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_ADD":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUser);
                    await InvokeEventAsync(GuildUserAdd, new(json, GetGuildId(), Rest), user => Cache = Cache.CacheGuildUser(user)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUser);
                    await InvokeEventAsync(GuildUserUpdate, new(json, GetGuildId(), Rest), user => Cache = Cache.CacheGuildUser(user)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBER_REMOVE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUserRemoveEventArgs);
                    await InvokeEventAsync(GuildUserRemove, new(json, Rest), args => Cache = Cache.RemoveGuildUser(args.GuildId, args.User.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_MEMBERS_CHUNK":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildUserChunkEventArgs);
                    await InvokeEventAsync(GuildUserChunk, new(json, Rest), args =>
                    {
                        var guildId = args.GuildId;
                        var cache = Cache.CacheGuildUsers(guildId, args.Users);
                        var presences = args.Presences;
                        if (presences is not null)
                            cache = cache.CachePresences(guildId, presences);
                        Cache = cache;
                    }).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleCreate, new(json.Role, json.GuildId, Rest), role => Cache = Cache.CacheRole(role)).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleEventArgs);
                    await InvokeEventAsync(RoleUpdate, new(json.Role, json.GuildId, Rest), role => Cache = Cache.CacheRole(role)).ConfigureAwait(false);
                }
                break;
            case "GUILD_ROLE_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonRoleDeleteEventArgs);
                    await InvokeEventAsync(RoleDelete, new(json), args => Cache = Cache.RemoveRole(args.GuildId, args.RoleId)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventCreate, new(json, Rest), scheduledEvent => Cache = Cache.CacheGuildScheduledEvent(scheduledEvent)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventUpdate, new(json, Rest), scheduledEvent => Cache = Cache.CacheGuildScheduledEvent(scheduledEvent)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonGuildScheduledEvent);
                    await InvokeEventAsync(GuildScheduledEventDelete, new(json, Rest), scheduledEvent => Cache = Cache.RemoveGuildScheduledEvent(scheduledEvent.GuildId, scheduledEvent.Id)).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_ADD":
                {
                    await InvokeEventAsync(GuildScheduledEventUserAdd, () => new(data.ToObject(Serialization.Default.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                {
                    await InvokeEventAsync(GuildScheduledEventUserRemove, () => new(data.ToObject(Serialization.Default.JsonGuildScheduledEventUserEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_CREATE":
                {
                    await InvokeEventAsync(GuildIntegrationCreate, () => new(new(data.ToObject(Serialization.Default.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_UPDATE":
                {
                    await InvokeEventAsync(GuildIntegrationUpdate, () => new(new(data.ToObject(Serialization.Default.JsonIntegration), Rest), GetGuildId())).ConfigureAwait(false);
                }
                break;
            case "INTEGRATION_DELETE":
                {
                    await InvokeEventAsync(GuildIntegrationDelete, () => new(data.ToObject(Serialization.Default.JsonGuildIntegrationDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "INTERACTION_CREATE":
                {
                    await InvokeEventAsync(InteractionCreate, () => Interaction.CreateFromJson(data.ToObject(Serialization.Default.JsonInteraction), Cache, Rest)).ConfigureAwait(false);
                }
                break;
            case "INVITE_CREATE":
                {
                    await InvokeEventAsync(GuildInviteCreate, () => new(data.ToObject(Serialization.Default.JsonGuildInvite), Rest)).ConfigureAwait(false);
                }
                break;
            case "INVITE_DELETE":
                {
                    await InvokeEventAsync(GuildInviteDelete, () => new(data.ToObject(Serialization.Default.JsonGuildInviteDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_CREATE":
                {
                    await InvokeEventAsync(
                        MessageCreate,
                        () => data.ToObject(Serialization.Default.JsonMessage),
                        json => Message.CreateFromJson(json, Cache, Rest),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_DMSemaphores!.TryGetValue(channelId, out var semaphore))
                                _DMSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_UPDATE":
                {
                    await InvokeEventAsync(
                        MessageUpdate,
                        () => data.ToObject(Serialization.Default.JsonMessage),
                        json => IPartialMessage.CreateFromJson(json, Cache, Rest),
                        json => _configuration.CacheDMChannels && !json.GuildId.HasValue && !json.Flags.GetValueOrDefault().HasFlag(MessageFlags.Ephemeral),
                        json =>
                        {
                            var channelId = json.ChannelId;
                            if (!_DMSemaphores!.TryGetValue(channelId, out var semaphore))
                                _DMSemaphores.Add(channelId, semaphore = new(1, 1));
                            return semaphore;
                        },
                        json => CacheChannelAsync(json.ChannelId)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE":
                {
                    await InvokeEventAsync(MessageDelete, () => new(data.ToObject(Serialization.Default.JsonMessageDeleteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_DELETE_BULK":
                {
                    await InvokeEventAsync(MessageDeleteBulk, () => new(data.ToObject(Serialization.Default.JsonMessageDeleteBulkEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_ADD":
                {
                    await InvokeEventAsync(MessageReactionAdd, () => new(data.ToObject(Serialization.Default.JsonMessageReactionAddEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE":
                {
                    await InvokeEventAsync(MessageReactionRemove, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_ALL":
                {
                    await InvokeEventAsync(MessageReactionRemoveAll, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveAllEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_REACTION_REMOVE_EMOJI":
                {
                    await InvokeEventAsync(MessageReactionRemoveEmoji, () => new(data.ToObject(Serialization.Default.JsonMessageReactionRemoveEmojiEventArgs))).ConfigureAwait(false);
                }
                break;
            case "PRESENCE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonPresence);
                    await InvokeEventAsync(PresenceUpdate, new(json, null, Rest), presence => Cache = Cache.CachePresence(presence)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_CREATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceCreate, new(json, Rest), stageInstance => Cache = Cache.CacheStageInstance(stageInstance)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceUpdate, new(json, Rest), stageInstance => Cache = Cache.CacheStageInstance(stageInstance)).ConfigureAwait(false);
                }
                break;
            case "STAGE_INSTANCE_DELETE":
                {
                    var json = data.ToObject(Serialization.Default.JsonStageInstance);
                    await InvokeEventAsync(StageInstanceDelete, new(json, Rest), stageInstance => Cache = Cache.RemoveStageInstance(stageInstance.GuildId, stageInstance.Id)).ConfigureAwait(false);
                }
                break;
            case "TYPING_START":
                {
                    await InvokeEventAsync(TypingStart, () => new(data.ToObject(Serialization.Default.JsonTypingStartEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "USER_UPDATE":
                {
                    await InvokeEventAsync(CurrentUserUpdate, new(data.ToObject(Serialization.Default.JsonUser), Rest), user => Cache = Cache.CacheCurrentUser(user)).ConfigureAwait(false);
                }
                break;
            case "VOICE_STATE_UPDATE":
                {
                    var json = data.ToObject(Serialization.Default.JsonVoiceState);
                    await InvokeEventAsync(VoiceStateUpdate, new(json, json.GuildId.GetValueOrDefault(), Rest), voiceState =>
                    {
                        if (voiceState.ChannelId.HasValue)
                            Cache = Cache.CacheVoiceState(voiceState);
                        else
                            Cache = Cache.RemoveVoiceState(voiceState.GuildId, voiceState.UserId);
                    }).ConfigureAwait(false);
                }
                break;
            case "VOICE_SERVER_UPDATE":
                {
                    await InvokeEventAsync(VoiceServerUpdate, () => new(data.ToObject(Serialization.Default.JsonVoiceServerUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "WEBHOOKS_UPDATE":
                {
                    await InvokeEventAsync(WebhooksUpdate, () => new(data.ToObject(Serialization.Default.JsonWebhooksUpdateEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_POLL_VOTE_ADD":
                {
                    await InvokeEventAsync(MessagePollVoteAdd, () => new(data.ToObject(Serialization.Default.JsonMessagePollVoteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "MESSAGE_POLL_VOTE_REMOVE":
                {
                    await InvokeEventAsync(MessagePollVoteRemove, () => new(data.ToObject(Serialization.Default.JsonMessagePollVoteEventArgs))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_CREATE":
                {
                    await InvokeEventAsync(EntitlementCreate, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_UPDATE":
                {
                    await InvokeEventAsync(EntitlementUpdate, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            case "ENTITLEMENT_DELETE":
                {
                    await InvokeEventAsync(EntitlementDelete, () => new(data.ToObject(Serialization.Default.JsonEntitlement))).ConfigureAwait(false);
                }
                break;
            case "GUILD_JOIN_REQUEST_UPDATE":
                {
                    await InvokeEventAsync(GuildJoinRequestUpdate, () => new(data.ToObject(Serialization.Default.JsonGuildJoinRequestUpdateEventArgs), Rest)).ConfigureAwait(false);
                }
                break;
            case "GUILD_JOIN_REQUEST_DELETE":
                {
                    await InvokeEventAsync(GuildJoinRequestDelete, () => new(data.ToObject(Serialization.Default.JsonGuildJoinRequestDeleteEventArgs))).ConfigureAwait(false);
                } 
                break;
            default:
                {
                    await InvokeEventAsync(UnknownEvent, () => new(name, data)).ConfigureAwait(false);
                }
                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong GetGuildId() => data.GetProperty("guild_id").ToObject(Serialization.Default.UInt64);

        async ValueTask CacheChannelAsync(ulong channelId)
        {
            var cache = Cache;
            if (!cache.DMChannels.ContainsKey(channelId))
            {
                var channel = await Rest.GetChannelAsync(channelId).ConfigureAwait(false);
                if (channel is DMChannel dMChannel)
                {
                    lock (_DMsLock!)
                        Cache = Cache.CacheDMChannel(dMChannel);
                }
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _compression.Dispose();
            if (_disposeRest)
                Rest.Dispose();
            Cache?.Dispose();
        }
        base.Dispose(disposing);
    }
}
