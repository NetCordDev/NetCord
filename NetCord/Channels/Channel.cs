using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal abstract partial class Channel(JsonChannel jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonChannel>, IInteractionChannel
{
    JsonChannel IJsonModel<JsonChannel>.JsonModel => _jsonModel;
    
    private protected JsonChannel _jsonModel = jsonModel;

    public override ulong Id => _jsonModel.Id;

    public ChannelFlags? Flags => _jsonModel.Flags;

    Permissions IInteractionChannel.Permissions => _jsonModel.Permissions.GetValueOrDefault();

    public override string ToString() => $"<#{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatChannel(destination, out charsWritten, Id);
}
