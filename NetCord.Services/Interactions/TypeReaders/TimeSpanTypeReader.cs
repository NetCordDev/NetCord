using System.Globalization;
using System.Text.RegularExpressions;

namespace NetCord.Services.Interactions.TypeReaders;

public class TimeSpanTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options)
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
            return Task.FromResult<object?>(new TimeSpan(
                checked((y.Success ? int.Parse(y.Value, NumberStyles.None, options.CultureInfo) * 365 : 0) + (d.Success ? int.Parse(d.Value, NumberStyles.None, options.CultureInfo) : 0)),
                h.Success ? int.Parse(h.Value, NumberStyles.None, options.CultureInfo) : 0,
                m.Success ? int.Parse(m.Value, NumberStyles.None, options.CultureInfo) : 0,
                s.Success ? int.Parse(s.Value, NumberStyles.None, options.CultureInfo) : 0));
        }
        else
            throw new FormatException($"Invalid {nameof(TimeSpan)}.");
    }
}
