using System.Text.RegularExpressions;

namespace NetCord.Services.Commands.TypeReaders;

public class TimeSpanTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options)
    {
        RegexOptions regexOptions = options.IgnoreCase ? RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.IgnoreCase
                                                       : RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft;
        // Regex blocks minus values
        var timeSpan = Regex.Match(input.ToString(), @"^((?<y>\d+)y)?((?<d>\d+)d)?((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?$", regexOptions);
        if (timeSpan.Success)
        {
            var y = timeSpan.Groups["y"];
            var d = timeSpan.Groups["d"];
            var h = timeSpan.Groups["h"];
            var m = timeSpan.Groups["m"];
            var s = timeSpan.Groups["s"];
            int days = checked((y.Success ? int.Parse(y.Value) * 365 : 0) + (d.Success ? int.Parse(d.Value) : 0));
            return Task.FromResult((object?)new TimeSpan(days, h.Success ? int.Parse(h.Value) : 0, m.Success ? int.Parse(m.Value) : 0, s.Success ? int.Parse(s.Value) : 0));
        }
        else
            throw new FormatException($"Invalid {nameof(TimeSpan)}.");
    }
}