using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Gateway.WebSockets;
using NetCord.JsonModels;

namespace HostingTest;

public abstract class GatewayHandlersTestBase
{
    protected class JsonGatewayMessage
    {
        [JsonPropertyName("op")]
        public GatewayOpcode Opcode { get; set; }

        [JsonPropertyName("d")]
        public JsonElement? Data { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public string? Event { get; set; }
    }

    private static JsonGatewayMessage CreateRateLimitedMessage(int seq)
    {
        return new()
        {
            SequenceNumber = seq,
            Event = "RATE_LIMITED",
            Data = JsonSerializer.SerializeToElement(new JsonRateLimitedEventArgs()
            {
                Opcode = GatewayOpcode.RequestGuildUsers,
                RetryAfter = 1,
                Metadata = JsonSerializer.SerializeToElement(new JsonRequestGuildUsersRateLimitMetadata()
                {
                    GuildId = 123,
                }),
            }),
            Opcode = GatewayOpcode.Dispatch,
        };
    }

    private static JsonGatewayMessage CreateApplicationCommandPermissionsUpdateMessage(int seq)
    {
        return new()
        {
            SequenceNumber = seq,
            Event = "APPLICATION_COMMAND_PERMISSIONS_UPDATE",
            Data = JsonSerializer.SerializeToElement(new JsonApplicationCommandGuildPermission()
            {
                Id = 123,
                Type = ApplicationCommandGuildPermissionType.Role,
                Permission = true,
            }),
            Opcode = GatewayOpcode.Dispatch,
        };
    }

    protected sealed class RateLimitedWebSocketConnection : MockWebSocketConnection
    {
        protected override JsonGatewayMessage CreateMessage(int seq) => CreateRateLimitedMessage(seq);
    }

    protected sealed class ByTurnsWebSocketConnection : MockWebSocketConnection
    {
        private int _turn;

        protected override JsonGatewayMessage CreateMessage(int seq)
        {
            return Interlocked.Increment(ref _turn) % 2 is 0
                ? CreateRateLimitedMessage(seq)
                : CreateApplicationCommandPermissionsUpdateMessage(seq);
        }
    }

    protected abstract partial class MockWebSocketConnection : IWebSocketConnection
    {
        private int _seq;

        private MemoryStream? _stream;

        private CancellationTokenSource? _cancellationTokenSource;

        public int? CloseStatus => null;

        public string? CloseStatusDescription => null;

        public void Abort()
        {
            _cancellationTokenSource?.Cancel();
        }

        public ValueTask CloseAsync(int closeStatus, string? closeStatusDescription, CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource?.Cancel();

            return default;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        public ValueTask OpenAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            _seq = 0;
            _cancellationTokenSource = new();

            return default;
        }

        protected abstract JsonGatewayMessage CreateMessage(int seq);

        public async ValueTask<WebSocketConnectionReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_stream is { } existingStream)
            {
                var existingResult = Copy(buffer, existingStream);

                if (existingResult.EndOfMessage)
                    _stream = null;

                return existingResult;
            }

            if (_cancellationTokenSource is { IsCancellationRequested: true })
                return new(0, WebSocketMessageType.Close, true);

            var message = CreateMessage(_seq++);

            MemoryStream stream = new();

            JsonSerializer.Serialize(new Utf8JsonWriter(stream), message);

            stream.Position = 0;

            var result = Copy(buffer, stream);

            if (!result.EndOfMessage)
                _stream = stream;

            return result;

            static WebSocketConnectionReceiveResult Copy(Memory<byte> buffer, MemoryStream stream)
            {
                var remainingBytes = (int)(stream.Length - stream.Position);

                var length = Math.Min(buffer.Length, remainingBytes);

                stream.ReadExactly(buffer.Span[..length]);

                return new(length, WebSocketMessageType.Text, remainingBytes <= buffer.Length);
            }
        }

        public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, WebSocketMessageFlags messageFlags, CancellationToken cancellationToken = default)
        {
            return default;
        }
    }

    protected sealed class MockWebSocketConnectionProvider<TConnection> : IWebSocketConnectionProvider where TConnection : IWebSocketConnection, new()
    {
        public IWebSocketConnection CreateConnection() => new TConnection();
    }
}

