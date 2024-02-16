using NetCord.Rest;

namespace NetCord;

public class ThreadUser(JsonModels.JsonThreadUser jsonModel, RestClient client) : ClientEntity(client), ISpanFormattable, IJsonModel<JsonModels.JsonThreadUser>
{
    JsonModels.JsonThreadUser IJsonModel<JsonModels.JsonThreadUser>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.UserId;
    public ulong ThreadId => jsonModel.ThreadId;
    public DateTimeOffset JoinTimestamp => jsonModel.JoinTimestamp;
    public int Flags => jsonModel.Flags;

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
