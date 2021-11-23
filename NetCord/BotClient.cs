using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using NetCord.WebSockets;

namespace NetCord
{
    public partial class BotClient
    {
        public SelfUser User { get; private set; }
        //public WebSocketState SocketState => websocket.State;

        //private readonly Cache _cache = new();
        internal Dictionary<DiscordId, Guild> _guilds = new();
        internal Dictionary<DiscordId, DMChannel> _DMChannels = new();
        internal Dictionary<DiscordId, GroupDMChannel> _groupDMChannels = new();

        public IEnumerable<Guild> Guilds => _guilds.Values.AsEnumerable();

        internal readonly string _botToken;
        internal readonly WebSocket _websocket = new(new(Discord.GatewayUrl));
        internal readonly HttpClient _httpClient = new();
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private readonly ClientConfig _config;

        public event Action Connecting;
        public event Action Connected;
        public event Action Disconnected;
        public event Action Closed;
        public event Action Ready;

        public event LogEventHandler Log;

        public event MessageReceivedEventHandler MessageReceived;
        public event InteractionCreatedEventHandler<MenuInteraction> MenuInteractionCreated;
        public event InteractionCreatedEventHandler<ButtonInteraction> ButtonInteractionCreated;
        //public event InteractionCreatedEventHandler<ApplicationCommandInteraction> ApplicationCommandInteractionCreated;

        public delegate void LogEventHandler(string text, LogType type);
        public delegate void MessageReceivedEventHandler(UserMessage message);
        public delegate void InteractionCreatedEventHandler<T>(T interaction);

        public string SessionId { get; private set; }
        public int SequenceNumber { get; private set; }

        public DiscordId? ApplicationId { get; private set; }
        public ApplicationFlags? ApplicationFlags { get; private set; }

        public BotClient(string token, TokenType tokenType)
        {
            SetupWebSocket();
            _botToken = token;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"{tokenType} {_botToken}");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NetCord");
            _config = new();
        }

        public BotClient(string token, TokenType tokenType, ClientConfig config) : this(token, tokenType)
        {
            _config = config;
        }

        private void SetupWebSocket()
        {
            _websocket.Connecting += () =>
            {
                LogInfo("Connecting", LogType.Gateway);
                try
                {
                    Connecting?.Invoke();
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
            };
            _websocket.Connected += () =>
            {
                _token = (_tokenSource = new()).Token;
                LogInfo("Connected", LogType.Gateway);
                try
                {
                    Connected?.Invoke();
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
            };
            _websocket.Disconnected += async (closeStatus, description) =>
            {
                _tokenSource.Cancel();
                if (string.IsNullOrEmpty(description))
                {
                    LogInfo("Disconnected", LogType.Gateway);
                    try
                    {
                        Disconnected?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                    try
                    {
                        await ResumeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                }
                else
                    LogInfo($"Disconnected because of: {description}", LogType.Exception);
            };
            _websocket.Closed += () =>
            {
                _tokenSource.Cancel();
                LogInfo("Closed", LogType.Gateway);
                try
                {
                    Closed?.Invoke();
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
            };
            _websocket.MessageReceived += (MemoryStream data) =>
            {
                var json = JsonDocument.Parse(data.ToArray());
                ProcessMessage(json);
            };
        }

        /// <summary>
        /// Connect the <see cref="BotClient"/> to gateway
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await _websocket.ConnectAsync().ConfigureAwait(false);
            await SendIdentifyAsync().ConfigureAwait(false);
        }

        private Task SendIdentifyAsync()
        {
            var authorizationMessage = @"{
  ""op"": 2,
  ""d"": {
    ""token"": """ + _botToken + @""",
    ""intents"": " + ((uint)_config.Intents) + @",
    ""properties"": {
      ""$os"": ""linux"",
      ""$browser"": ""NetCord"",
      ""$device"": ""NetCord""
    },
    ""large_threshold"": 250
  }
}";
            return _websocket.SendAsync(authorizationMessage, _token);
        }

        private async Task ResumeAsync()
        {
            await _websocket.ConnectAsync().ConfigureAwait(false);

            var resumeMessage = @"{
  ""op"": 6,
  ""d"": {
    ""token"": """ + _botToken + @""",
    ""session_id"": """ + SessionId + @""",
    ""seq"": " + SequenceNumber + @"
  }
}";
            await _websocket.SendAsync(resumeMessage, _token).ConfigureAwait(false);
        }

        private void LogInfo(string text, LogType type)
        {
            try
            {
                Log?.Invoke(text, type);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Disconnect the <see cref="BotClient"/> from gateway
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            _tokenSource.Cancel();
            await _websocket.CloseAsync().ConfigureAwait(false);
        }

        private async void BeginHeartbeatAsync(JsonElement message)
        {
            var time = message.GetProperty("d").GetProperty("heartbeat_interval").GetInt32();
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(time));
            while (true)
            {
                try
                {
                    await timer.WaitForNextTickAsync(_token).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
                await _websocket.SendAsync(@"{
  ""op"": 1,
  ""d"": " + (SequenceNumber.ToString() ?? "null") + @"
}", _token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates <see cref="SequenceNumber"/>
        /// </summary>
        /// <param name="element"></param>
        private void UpdateSequenceNumber(JsonElement element)
        {
            SequenceNumber = element.GetProperty("s").GetInt32();
        }

        public Guild GetGuild(DiscordId id)
        {
            if (TryGetGuild(id, out Guild guild))
                return guild;
            throw new EntityNotFoundException("Guild not found");
        }

        public bool TryGetGuild(DiscordId id, [NotNullWhen(true)] out Guild? guild)
        {
            if (id == null)
            {
                guild = null;
                return false;
            }
            return _guilds.TryGetValue(id, out guild);
        }

        public DMChannel GetDMChannel(DiscordId channelId)
        {
            if (TryGetDMChannel(channelId, out DMChannel channel))
                return channel;
            throw new EntityNotFoundException("Channel not found");
        }

        public bool TryGetDMChannel(DiscordId channelId, [NotNullWhen(true)] out DMChannel channel)
        {
            if (!_DMChannels.TryGetValue(channelId, out channel) && _groupDMChannels.TryGetValue(channelId, out GroupDMChannel groupDMChannel))
                channel = groupDMChannel;
            return true;
        }

        public Task<Channel> GetChannelAsync(DiscordId channelId) => ChannelHelper.GetChannelAsync(this, channelId);
    }
}
