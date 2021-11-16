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

        //private readonly DiscordSocketClientConfig config;

        public event Func<Task> Connecting;
        public event Func<Task> Connected;
        public event Func<Task> Disconnected;
        public event Func<Task> Closed;
        public event Func<Task> Ready;

        public event Func<string, LogType, Task> Log;

        public event Func<UserMessage, Task> MessageReceived;
        public event Func<MenuInteraction, Task> MenuInteractionCreated;
        public event Func<ButtonInteraction, Task> ButtonInteractionCreated;
        public event Func<ApplicationCommandInteraction, Task> ApplicationCommandInteractionCreated;

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
        }

        private void SetupWebSocket()
        {
            _websocket.Connecting += () =>
            {
                try
                {
                    Log?.Invoke("Connecting", LogType.Gateway);
                }
                finally
                {
                    try
                    {
                        Connecting?.Invoke();
                    }
                    catch
                    {
                    }
                }
                return Task.CompletedTask;
            };
            _websocket.Connected += () =>
            {
                _tokenSource = new();
                try
                {
                    _ = Log?.Invoke("Connected", LogType.Gateway);
                }
                finally
                {
                    try
                    {
                        _ = Connected?.Invoke();
                    }
                    catch
                    {

                    }
                }
                return Task.CompletedTask;
            };
            _websocket.Disconnected += () =>
            {
                _tokenSource.Cancel();
                try
                {
                    Log?.Invoke("Disconnected", LogType.Gateway);
                }
                finally
                {
                    try
                    {
                        Disconnected?.Invoke();
                    }
                    finally
                    {
                        try
                        {
                            _ = ResumeAsync();
                        }
                        catch
                        {
                        }
                    }
                }
                return Task.CompletedTask;
            };
            _websocket.Closed += () =>
            {
                _tokenSource.Cancel();
                try
                {
                    Log?.Invoke("Closed", LogType.Gateway);
                }
                finally
                {
                    try
                    {
                        Closed?.Invoke();
                    }
                    catch
                    {
                    }
                }
                return Task.CompletedTask;
            };
            _websocket.MessageReceived += (MemoryStream stream) =>
            {
                var json = JsonDocument.Parse(stream.ToArray());
                ProcessMessage(json);
                return Task.CompletedTask;
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
    ""intents"": 32767,
    ""properties"": {
      ""$os"": ""linux"",
      ""$browser"": ""NetCord"",
      ""$device"": ""NetCord""
    },
    ""large_threshold"": 250
  }
}";
            return _websocket.SendAsync(authorizationMessage);
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
            await _websocket.SendAsync(resumeMessage).ConfigureAwait(false);
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
                    await timer.WaitForNextTickAsync(_tokenSource.Token).ConfigureAwait(false);
                }
                catch
                {
                    return;
                }
                await _websocket.SendAsync(@"{
  ""op"": 1,
  ""d"": " + (SequenceNumber.ToString() ?? "null") + @"
}", _tokenSource.Token).ConfigureAwait(false);
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
