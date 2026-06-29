namespace NetCord.Services.ApplicationCommands;

public readonly record struct RegisteredApplicationCommand<TContext>(ulong Id, ApplicationCommandInfo<TContext> Info) : ISpanFormattable where TContext : IApplicationCommandContext
{
    public override string ToString() => $"</{Info.Name}:{Id}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) =>
        Mention.TryFormatSlashCommand(destination, out charsWritten, Id, Info.Name);
}
