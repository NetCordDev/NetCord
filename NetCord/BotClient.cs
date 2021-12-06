using System.Text.Json;

using NetCord.WebSockets;

namespace NetCord
{
    public partial class BotClient : Entity
    {
        private readonly string _botToken;
        private readonly WebSocket _webSocket = new(new(Discord.GatewayUrl));
        private readonly ClientConfig _config;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private int _connectDelay = 0;

        internal readonly HttpClient _httpClient = new();
        internal Dictionary<DiscordId, Guild> _guilds = new();
        internal Dictionary<DiscordId, DMChannel> _DMChannels = new();
        internal Dictionary<DiscordId, GroupDMChannel> _groupDMChannels = new();

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

        public SelfUser User { get; private set; }
        public override DiscordId Id => User.Id;
        public string SessionId { get; private set; }
        public int SequenceNumber { get; private set; }
        public DiscordId? ApplicationId { get; private set; }
        public ApplicationFlags? ApplicationFlags { get; private set; }

        public IReadOnlyDictionary<DiscordId, Guild> Guilds
        {
            get
            {
                lock (_guilds)
                    return new Dictionary<DiscordId, Guild>(_guilds);
            }
        }

        public IReadOnlyDictionary<DiscordId, DMChannel> DMChannels
        {
            get
            {
                lock (_DMChannels)
                    return new Dictionary<DiscordId, DMChannel>(_DMChannels);
            }
        }

        public IReadOnlyDictionary<DiscordId, GroupDMChannel> GroupDMChannels
        {
            get
            {
                lock (_groupDMChannels)
                    return new Dictionary<DiscordId, GroupDMChannel>(_groupDMChannels);
            }
        }

        public BotClient(string token, TokenType tokenType)
        {
            ArgumentNullException.ThrowIfNull(token, nameof(token));
            SetupWebSocket();
            _botToken = token;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"{tokenType} {_botToken}");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("NetCord");
            _config = new();
        }

        public BotClient(string token, TokenType tokenType, ClientConfig? config) : this(token, tokenType)
        {
            if (config != null)
                _config = config;
        }

        private void SetupWebSocket()
        {
            _webSocket.Connecting += () =>
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
            _webSocket.Connected += () =>
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
            _webSocket.Disconnected += async (closeStatus, description) =>
            {
                _tokenSource.Cancel();
                LogInfo("Disconnected", LogType.Gateway);
                try
                {
                    Disconnected?.Invoke();
                }
                catch (Exception ex)
                {
                    LogInfo(ex.Message, LogType.Exception);
                }
                if (!string.IsNullOrEmpty(description))
                    LogInfo(description, LogType.Exception);

                while (true)
                {
                    if (_connectDelay > 0)
                    {
                        await Task.Delay(_connectDelay).ConfigureAwait(false);
                        if (_connectDelay < 960_000)
                            _connectDelay *= 2;
                    }
                    else
                        _connectDelay = 30_000;
                    try
                    {
                        await ResumeAsync().ConfigureAwait(false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.Message, LogType.Exception);
                    }
                }
            };
            _webSocket.Closed += () =>
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
            _webSocket.MessageReceived += (MemoryStream data) =>
            {
                var json = JsonDocument.Parse(data.ToArray());
                //Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true }));
                ProcessMessage(json);
            };
        }

        /// <summary>
        /// Connect the <see cref="BotClient"/> to gateway
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await _webSocket.ConnectAsync().ConfigureAwait(false);
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
            return _webSocket.SendAsync(authorizationMessage, _token);
        }

        private async Task ResumeAsync()
        {
            await _webSocket.ConnectAsync().ConfigureAwait(false);

            var resumeMessage = @"{
  ""op"": 6,
  ""d"": {
    ""token"": """ + _botToken + @""",
    ""session_id"": """ + SessionId + @""",
    ""seq"": " + SequenceNumber + @"
  }
}";
            await _webSocket.SendAsync(resumeMessage, _token).ConfigureAwait(false);
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
            await _webSocket.CloseAsync().ConfigureAwait(false);
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
                    await _webSocket.SendAsync($"{{\"op\":1,\"d\":{SequenceNumber}}}", _token).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
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

        public Task<Channel> GetChannelAsync(DiscordId channelId) => ChannelHelper.GetChannelAsync(this, channelId);
    }
}
