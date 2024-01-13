using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildEmoji : Emoji, ISpanFormattable
{
    private protected readonly RestClient _client;

    public GuildEmoji(JsonEmoji jsonModel, ulong guildId, RestClient client) : base(jsonModel)
    {
        _client = client;

        var creator = jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);

        GuildId = guildId;
    }

    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    public IReadOnlyList<ulong>? AllowedRoles => _jsonModel.AllowedRoles;

    public User? Creator { get; }

    public bool? RequireColons => _jsonModel.RequireColons;

    public bool? Managed => _jsonModel.Managed;

    public bool? Available => _jsonModel.Available;

    public ulong GuildId { get; }

    public override string ToString() => Animated ? $"<a:{Name}:{Id}>" : $"<:{Name}:{Id}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var name = Name;
        if (Animated)
        {
            if (destination.Length < 6 + name.Length || !Id.TryFormat(destination[(4 + name.Length)..^1], out int length))
            {
                charsWritten = 0;
                return false;
            }

            "<a:".CopyTo(destination);
            name.CopyTo(destination[3..]);
            destination[3 + name.Length] = ':';
            destination[4 + name.Length + length] = '>';
            charsWritten = 5 + name.Length + length;
            return true;
        }
        else
        {
            if (destination.Length < 5 + name.Length || !Id.TryFormat(destination[(3 + name.Length)..^1], out int length))
            {
                charsWritten = 0;
                return false;
            }

            "<:".CopyTo(destination);
            name.CopyTo(destination[2..]);
            destination[2 + name.Length] = ':';
            destination[3 + name.Length + length] = '>';
            charsWritten = 4 + name.Length + length;
            return true;
        }
    }

    #region Guild
    public Task<GuildEmoji> ModifyAsync(Action<GuildEmojiOptions> action, RequestProperties? properties = null) => _client.ModifyGuildEmojiAsync(GuildId, Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildEmojiAsync(GuildId, Id, properties);
    #endregion
}
