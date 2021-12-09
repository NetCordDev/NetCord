using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord
{
    public partial class BotClient
    {
        /// <summary>
        /// Runs an action based on a <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async void ProcessMessage(JsonDocument message)
        {
            var rootElement = message.RootElement;
            switch (rootElement.GetProperty("op").GetInt32())
            {
                case 0:
                    UpdateSequenceNumber(rootElement);
                    ProcessEvent(rootElement);
                    break;
                //case 1: break;
                //case 2: break;
                //case 3: break;
                //case 4: break;
                //case 5: break;
                //case 6: break;
                case 7:
                    LogInfo("Reconnect request", LogType.Gateway);
                    await _webSocket.CloseAsync().ConfigureAwait(false);
                    _ = ResumeAsync();
                    break;
                //case 8: break;
                case 9:
                    LogInfo("Invalid session", LogType.Gateway);
                    _ = SendIdentifyAsync();
                    break;
                case 10:
                    BeginHeartbeatAsync(rootElement);
                    break;
                    //case 11: break;
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
                // guild events
                case "GUILD_CREATE":
                    AddOrUpdate(jsonElement.GetProperty("d"), _guilds, (JsonGuild j, BotClient c) => new Guild(j, c));
                    break;
                case "GUILD_UPDATE":
                    AddOrUpdate(jsonElement.GetProperty("d"), _guilds, (JsonGuild j, Guild g, BotClient c) =>
                    {
                        return new(j with { CreatedAt = g.CreatedAt, IsLarge = g.IsLarge, MemberCount = g.MemberCount }, c)
                        {
                            _voiceStates = g._voiceStates,
                            _users = g._users,
                            _channels = g._channels,
                            _activeThreads = g._activeThreads,
                            _presences = g._presences,
                        };
                    });
                    break;
                case "GUILD_DELETE":
                    TryRemove(jsonElement.GetProperty("d"), _guilds);
                    break;
                case "GUILD_ROLE_CREATE":
                case "GUILD_ROLE_UPDATE":
                    var property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out Guild g))
                        AddOrUpdate(property.GetProperty("role"), g._roles, (JsonRole r, BotClient c) => new Role(r, c));
                    break;
                case "GUILD_ROLE_DELETE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        TryRemove(property, g._roles, "role_id");
                    break;
                case "CHANNEL_CREATE":
                case "CHANNEL_UPDATE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        AddOrUpdate<JsonChannel, IGuildChannel>(property, g._channels, (ch, c) => (IGuildChannel)Channel.CreateFromJson(ch, c));
                    break;
                case "CHANNEL_DELETE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        TryRemove(property, g._channels);
                    break;
                // also DM
                case "CHANNEL_PINS_UPDATE": break;
                case "THREAD_CREATE":
                case "THREAD_UPDATE":
                    property = jsonElement.GetProperty("d"); if (TryGetGuild(property, out g))
                        AddOrUpdate(property, g._activeThreads, (JsonChannel ch, BotClient c) => (Thread)Channel.CreateFromJson(ch, c)); break;
                case "THREAD_DELETE":
                    property = jsonElement.GetProperty("d"); if (TryGetGuild(property, out g))
                        TryRemove(property, g._activeThreads); break;
                case "THREAD_LIST_SYNC": break;
                case "THREAD_MEMBER_UPDATE": break;
                case "THREAD_MEMBERS_UPDATE ": break;
                case "STAGE_INSTANCE_CREATE":
                case "STAGE_INSTANCE_UPDATE":
                    property = jsonElement.GetProperty("d"); if (TryGetGuild(property, out g))
                        AddOrUpdate(property, g._stageInstances, (JsonStageInstance i, BotClient c) => new StageInstance(i, c)); break;
                case "STAGE_INSTANCE_DELETE":
                    property = jsonElement.GetProperty("d"); if (TryGetGuild(property, out g))
                        TryRemove(property, g._stageInstances); break;

                // guild members
                case "GUILD_MEMBER_ADD":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                    {
                        g.MemberCount++;
                        g.ApproximateMemberCount++;
                        AddOrUpdate(property, g._users, (JsonGuildUser u, BotClient c) => new GuildUser(u, g, c));
                    }
                    break;
                case "GUILD_MEMBER_UPDATE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        AddOrUpdate(property, g._users, (JsonGuildUser u, BotClient c) => new GuildUser(u, g, c));
                    break;
                case "GUILD_MEMBER_REMOVE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        TryRemove(property, g._users); break;
                case "THREAD_MEMBERS_UPDATE": break;

                // guild bans
                case "GUILD_BAN_ADD": break;
                case "GUILD_BAN_REMOVE": break;

                //guild emojis
                case "GUILD_EMOJIS_UPDATE": break;

                // guild integrations
                case "GUILD_INTEGRATIONS_UPDATE": break;
                case "INTEGRATION_CREATE": break;
                case "INTEGRATION_UPDATE": break;
                case "INTEGRATION_DELETE": break;

                // guild webhooks
                case "WEBHOOKS_UPDATE": break;

                // guild invites
                case "INVITE_CREATE": break;
                case "INVITE_DELETE": break;

                // guild voice states
                case "VOICE_STATE_UPDATE":
                    property = jsonElement.GetProperty("d");
                    if (TryGetGuild(property, out g))
                        AddUpdateOrDelete(property, g._voiceStates); break;

                // guild presences
                case "PRESENCE_UPDATE": break;

                // guild and DM messages
                case "MESSAGE_CREATE":
                    property = jsonElement.GetProperty("d");
                    var jsonMessage = property.ToObject<JsonMessage>();
                    var channelId = jsonMessage.ChannelId;
                    if (!_DMChannels.ContainsKey(channelId) && !_groupDMChannels.ContainsKey(channelId))
                    {
                        var channel = await GetChannelAsync(channelId).ConfigureAwait(false);
                        if (channel is GroupDMChannel groupDMChannel)
                            _groupDMChannels[channelId] = groupDMChannel;
                        else if (channel is DMChannel dMChannel)
                            _DMChannels[channelId] = dMChannel;
                    }
                    try
                    {
                        MessageReceived?.Invoke(new UserMessage(jsonMessage, this));
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    break;
                case "MESSAGE_UPDATE": break;
                case "MESSAGE_DELETE": break;
                case "MESSAGE_DELETE_BULK": break;

                // guild and DM message reactions
                case "MESSAGE_REACTION_ADD": break;
                case "MESSAGE_REACTION_REMOVE": break;
                case "MESSAGE_REACTION_REMOVE_ALL": break;
                case "MESSAGE_REACTION_REMOVE_EMOJI": break;

                // guild and DM message typing
                case "TYPING_START": break;

                case "READY":
                    _connectDelay = 0;
                    var d = jsonElement.GetProperty("d");
                    Ready ready = new(d.ToObject<JsonReady>(), this);
                    User = ready.User;
                    foreach (var channel in ready.DMChannels)
                    {
                        if (channel is GroupDMChannel groupDm)
                            _groupDMChannels.Add(groupDm.Id, groupDm);
                        else if (channel is DMChannel dm)
                            _DMChannels.Add(dm.Id, dm);
                    }
                    SessionId = ready.SessionId;
                    ApplicationId = ready.Application?.Id;
                    ApplicationFlags = ready.Application?.Flags;
                    LogInfo("Ready", LogType.Gateway);
                    try
                    {
                        Ready?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    break;
                case "INTERACTION_CREATE":
                    InvokeInteractionCreated(jsonElement.GetProperty("d"));
                    break;
                case "RESUMED":
                    LogInfo("Resumed previous session", LogType.Gateway);
                    break;
                case "USER_UPDATE":
                    User = new(jsonElement.GetProperty("d").ToObject<JsonUser>(), this);
                    break;
            }
        }

        private bool TryGetGuild(JsonElement jsonElement, [NotNullWhen(true)] out Guild? guild) => Guilds.TryGetValue(jsonElement.GetProperty("guild_id").ToObject<DiscordId>(), out guild);

        private void AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, Dictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, BotClient, TType> constructor) where TType : IEntity
        {
            var jsonObj = jsonElement.ToObject<TJsonType>();
            TType obj = constructor(jsonObj, this);
            lock (propertyToUpdate)
                propertyToUpdate[obj.Id] = obj;
        }

        private void AddOrUpdate<TJsonType, TType>(JsonElement jsonElement, Dictionary<DiscordId, TType> propertyToUpdate, Func<TJsonType, TType, BotClient, TType> constructor) where TType : IEntity where TJsonType : JsonEntity
        {
            var jsonObj = jsonElement.ToObject<TJsonType>();
            lock (propertyToUpdate)
            {
                TType obj = constructor(jsonObj, propertyToUpdate[jsonObj.Id], this);
                propertyToUpdate[obj.Id] = obj;
            }
        }

        private static void AddUpdateOrDelete(JsonElement jsonElement, Dictionary<DiscordId, VoiceState> propertyToUpdate)
        {
            JsonVoiceState obj = jsonElement.ToObject<JsonVoiceState>();
            if (obj.ChannelId == null)
            {
                lock (propertyToUpdate)
                    propertyToUpdate.Remove(obj.UserId);
            }
            else
            {
                lock (propertyToUpdate)
                    propertyToUpdate[obj.UserId] = new(obj);
            }
        }

        private static void TryRemove<T>(JsonElement jsonElement, Dictionary<DiscordId, T> propertyToUpdate, string propertyName = "id")
        {
            DiscordId id = jsonElement.GetProperty(propertyName).ToObject<DiscordId>();
            lock (propertyToUpdate)
                propertyToUpdate.Remove(id);
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

        private static void TryRemove(JsonElement jsonElement, Dictionary<DiscordId, GuildUser> propertyToUpdate)
        {
            DiscordId id = jsonElement.GetProperty("user").GetProperty("id").ToObject<DiscordId>();
            lock (propertyToUpdate)
                propertyToUpdate.Remove(id);
        }

        private void InvokeInteractionCreated(JsonElement jsonPart)
        {
            JsonInteraction interaction = jsonPart.ToObject<JsonInteraction>();
            var type = interaction.Type;
            if (type == InteractionType.ApplicationCommand)
            {
                //if (ApplicationCommandInteractionCreated != null)
                //{

                //}
            }
            else if (type == InteractionType.MessageComponent)
            {
                var componentType = interaction.Data.ComponentType;
                if (componentType == MessageComponentType.Button)
                {
                    try
                    {
                        ButtonInteractionCreated?.Invoke(new ButtonInteraction(interaction, this));
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                }
                else if (componentType == MessageComponentType.Menu)
                {
                    try
                    {
                        MenuInteractionCreated?.Invoke(new MenuInteraction(interaction, this));
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                }
            }
        }
    }
}
