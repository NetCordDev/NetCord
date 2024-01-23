using NetCord.Rest;

namespace NetCord;

public class ThreadUser : ClientEntity, ISpanFormattable, IJsonModel<JsonModels.JsonThreadUser>
{
    JsonModels.JsonThreadUser IJsonModel<JsonModels.JsonThreadUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadUser _jsonModel;

    public override ulong Id => _jsonModel.UserId;
    public ulong ThreadId => _jsonModel.ThreadId;
    public DateTimeOffset JoinTimestamp => _jsonModel.JoinTimestamp;
    public int Flags => _jsonModel.Flags;

    public ThreadUser(JsonModels.JsonThreadUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
