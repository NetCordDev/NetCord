using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord
{
    public partial class GatewayClient
    {
        /// <summary>
        /// Runs an action based on a <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void ProcessMessage(JsonDocument message)
        {
            var rootElement = message.RootElement;
            switch ((GatewayOpcode)rootElement.GetProperty("op").GetByte())
            {
                case GatewayOpcode.Dispatch:
                    UpdateSequenceNumber(rootElement);
                    ProcessEvent(rootElement);
                    break;
                case GatewayOpcode.Heartbeat: break;
                case GatewayOpcode.Reconnect:
                    LogInfo("Reconnect request", LogType.Gateway);
                    //await _webSocket.CloseAsync().ConfigureAwait(false);
                    _ = ResumeAsync();
                    break;
                case GatewayOpcode.InvalidSession:
                    LogInfo("Invalid session", LogType.Gateway);
                    _ = SendIdentifyAsync();
                    break;
                case GatewayOpcode.Hello:
                    BeginHeartbeatAsync(rootElement);
                    break;
                case GatewayOpcode.HeartbeatACK:
                    Latency = _latencyTimer.Elapsed;
                    break;
            }
        }

        /// <summary>
        /// Runs an action based on a <paramref name="jsonElement"/>
        /// </summary>
        /// <param name="jsonElement"></param>
        /// <returns></returns>
        private async void ProcessEvent(JsonElement jsonElement)
        {
            switch (jsonElement.GetProperty("t").GetString())
            {
                case "READY":
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    Ready ready = new(GetD().ToObject<JsonReady>(), this);
                    User = ready.User;
                    foreach (var channel in ready.DMChannels)
                    {
                        if (channel is GroupDMChannel groupDm)
                            _groupDMChannels = _groupDMChannels.Add(groupDm.Id, groupDm);
                        else if (channel is DMChannel dm)
                            _DMChannels = _DMChannels.Add(dm.Id, dm);
                    }
                    SessionId = ready.SessionId;
                    Application = ready.Application;
                    LogInfo("Ready", LogType.Gateway);
                    if (!_readyCompletionSource.Task.IsCompleted)
                        _readyCompletionSource.SetResult();

                    try
                    {
                        Ready?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    break;
                case "RESUMED":
                    Latency = _latencyTimer.Elapsed;
                    _reconnectTimer.Reset();
                    LogInfo("Resumed previous session", LogType.Gateway);
                    break;
                case "CHANNEL_CREATE":
                case "CHANNEL_UPDATE":
                    var property = GetD();
                    if (TryGetGuild(property, out var g))
                        AddOrUpdate<JsonChannel, IGuildChannel>(property, ref g._channels, (ch, c) => (IGuildChannel)Channel.CreateFromJson(ch, c));
                    break;
                case "CHANNEL_DELETE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        TryRemove(property, ref g._channels);
                    break;
                case "CHANNEL_PINS_UPDATE": break;
                case "THREAD_CREATE":
                case "THREAD_UPDATE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property, ref g._activeThreads, (JsonChannel ch, RestClient c) => (Thread)Channel.CreateFromJson(ch, c)); break;
                case "THREAD_DELETE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        TryRemove(property, ref g._activeThreads); break;
                case "THREAD_LIST_SYNC": break;
                case "THREAD_MEMBER_UPDATE": break;
                case "THREAD_MEMBERS_UPDATE ": break;
                case "GUILD_CREATE":
                    AddOrUpdate(GetD(), ref _guilds, (JsonGuild j, RestClient c) => new Guild(j, c));
                    break;
                case "GUILD_UPDATE":
                    AddOrUpdate(GetD(), ref _guilds, (JsonGuild j, Guild g, RestClient c) =>
                    {
                        return new(j with { CreatedAt = g.CreatedAt, IsLarge = g.IsLarge, MemberCount = g.MemberCount }, c)
                        {
                            _voiceStates = g._voiceStates,
                            _users = g._users,
                            _channels = g._channels,
                            _activeThreads = g._activeThreads,
                            _stageInstances = g._stageInstances,
                            _presences = g._presences,
                            _scheduledEvents = g._scheduledEvents,
                        };
                    });
                    break;
                case "GUILD_DELETE":
                    TryRemove(GetD(), ref _guilds);
                    break;
                case "GUILD_BAN_ADD": break;
                case "GUILD_BAN_REMOVE": break;
                case "GUILD_EMOJIS_UPDATE": break;
                case "GUILD_STICKERS_UPDATE": break;
                case "GUILD_INTEGRATIONS_UPDATE": break;
                case "GUILD_MEMBER_ADD":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property, ref g._users, (JsonGuildUser u, RestClient c) => new GuildUser(u, g, c));
                    break;
                case "GUILD_MEMBER_UPDATE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property, ref g._users, (JsonGuildUser u, RestClient c) => new GuildUser(u, g, c));
                    break;
                case "GUILD_MEMBER_REMOVE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        TryRemove(property, ref g._users); break;
                case "GUILD_MEMBERS_CHUNK": break;
                case "GUILD_ROLE_CREATE":
                case "GUILD_ROLE_UPDATE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property.GetProperty("role"), ref g._roles, (JsonGuildRole r, RestClient c) => new GuildRole(r, c));
                    break;
                case "GUILD_ROLE_DELETE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        TryRemove(property, ref g._roles, "role_id");
                    break;
                case "GUILD_SCHEDULED_EVENT_CREATE": break;
                case "GUILD_SCHEDULED_EVENT_UPDATE": break;
                case "GUILD_SCHEDULED_EVENT_DELETE": break;
                case "GUILD_SCHEDULED_EVENT_USER_ADD": break;
                case "GUILD_SCHEDULED_EVENT_USER_REMOVE": break;
                case "INTEGRATION_CREATE": break;
                case "INTEGRATION_UPDATE": break;
                case "INTEGRATION_DELETE": break;
                case "INTERACTION_CREATE":
                    JsonInteraction interaction = GetD().ToObject<JsonInteraction>();
                    var type = interaction.Type;
                    try
                    {
                        InteractionCreated?.Invoke(Interaction.CreateFromJson(interaction, this));
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    break;
                case "INVITE_CREATE": break;
                case "INVITE_DELETE": break;
                case "MESSAGE_CREATE":
                    property = GetD();
                    var jsonMessage = property.ToObject<JsonMessage>();
                    if (jsonMessage.GuildId == null)
                    {
                        var channelId = jsonMessage.ChannelId;
                        if (!_DMChannels.ContainsKey(channelId) && !_groupDMChannels.ContainsKey(channelId))
                        {
                            var channel = await Rest.Channel.GetAsync(channelId).ConfigureAwait(false);
                            if (channel is GroupDMChannel groupDMChannel)
                                _groupDMChannels = _groupDMChannels.SetItem(channelId, groupDMChannel);
                            else if (channel is DMChannel dMChannel)
                                _DMChannels = _DMChannels.SetItem(channelId, dMChannel);
                        }
                    }
                    try
                    {
                        MessageReceived?.Invoke(new(jsonMessage, this));
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    break;
                case "MESSAGE_UPDATE": break;
                case "MESSAGE_DELETE": break;
                case "MESSAGE_DELETE_BULK": break;
                case "MESSAGE_REACTION_ADD": break;
                case "MESSAGE_REACTION_REMOVE": break;
                case "MESSAGE_REACTION_REMOVE_ALL": break;
                case "MESSAGE_REACTION_REMOVE_EMOJI": break;
                case "PRESENCE_UPDATE": break;
                case "STAGE_INSTANCE_CREATE":
                case "STAGE_INSTANCE_UPDATE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property, ref g._stageInstances, (JsonStageInstance i, RestClient c) => new StageInstance(i, c)); break;
                case "STAGE_INSTANCE_DELETE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        TryRemove(property, ref g._stageInstances); break;
                case "TYPING_START": break;
                case "USER_UPDATE":
                    User = new(GetD().ToObject<JsonUser>(), Rest);
                    break;
                case "VOICE_STATE_UPDATE":
                    property = GetD();
                    if (TryGetGuild(property, out g))
                        AddUpdateOrDelete(property, ref g._voiceStates); break;
                case "VOICE_SERVER_UPDATE": break;
                case "WEBHOOKS_UPDATE": break;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            JsonElement GetD() => jsonElement.GetProperty("d");
        }

        private bool TryGetGuild(JsonElement jsonElement, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(jsonElement.GetProperty("guild_id").ToObject<DiscordId>(), out guild);

        private TType AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, RestClient, TType> constructor) where TType : IEntity
        {
            var jsonObj = jsonElement.ToObject<TJsonType>();
            TType obj = constructor(jsonObj, Rest);
            var v = propertyToUpdate.SetItem(obj.Id, obj);
            lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.SetItem(obj.Id, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
            return obj;
        }

        private TType AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, TType, RestClient, TType> constructor) where TType : IEntity where TJsonType : JsonEntity
        {
            var jsonObj = jsonElement.ToObject<TJsonType>();
            lock (propertyToUpdate)
            {
                TType obj = constructor(jsonObj, propertyToUpdate[jsonObj.Id], Rest);
                var oldP = propertyToUpdate;
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.SetItem(obj.Id, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                return obj;
            }
        }

        private static VoiceState? AddUpdateOrDelete(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, VoiceState> propertyToUpdate)
        {
            var jsonObj = jsonElement.ToObject<JsonVoiceState>();
            if (jsonObj.ChannelId == null)
            {
                lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                    propertyToUpdate = propertyToUpdate.Remove(jsonObj.UserId);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                return null;
            }
            else
            {
                VoiceState obj = new(jsonObj);
                lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                    propertyToUpdate = propertyToUpdate.SetItem(jsonObj.UserId, obj);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                return obj;
            }
        }

        private static void TryRemove<T>(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, T> propertyToUpdate, string propertyName = "id")
        {
            DiscordId id = jsonElement.GetProperty(propertyName).ToObject<DiscordId>();
            lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.Remove(id);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
        }

        //private void AddOrUpdate(JsonElement jsonElement, Dictionary<DiscordId, GuildUser> propertyToUpdate)
        //{
        //    var jsonObj = jsonElement.ToObject<JsonGuildUser>();
        //    GuildUser obj = new(jsonObj, null, this);
        //    DiscordId id = obj.Id;
        //    obj.VoiceState = propertyToUpdate[id].VoiceState;
        //    lock (propertyToUpdate)
        //        propertyToUpdate[id] = obj;
        //}

        private static void TryRemove(JsonElement jsonElement, ref ImmutableDictionary<DiscordId, GuildUser> propertyToUpdate)
        {
            DiscordId id = jsonElement.GetProperty("user").GetProperty("id").ToObject<DiscordId>();
            lock (propertyToUpdate)
#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
                propertyToUpdate = propertyToUpdate.Remove(id);
#pragma warning restore CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement
        }
    }
}
