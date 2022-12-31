using System.Globalization;
using System.Text.RegularExpressions;

namespace NetCord.Services.Commands.TypeReaders;

public class TimeSpanTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration)
    {
        RegexOptions regexOptions = configuration.IgnoreCase ? RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase
                                                             : RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant;
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
                checked((y.Success ? int.Parse(y.Value, NumberStyles.None, configuration.CultureInfo) * 365 : 0) + (d.Success ? int.Parse(d.Value, NumberStyles.None, configuration.CultureInfo) : 0)),
                h.Success ? int.Parse(h.Value, NumberStyles.None, configuration.CultureInfo) : 0,
                m.Success ? int.Parse(m.Value, NumberStyles.None, configuration.CultureInfo) : 0,
                s.Success ? int.Parse(s.Value, NumberStyles.None, configuration.CultureInfo) : 0));
        }
        else
            throw new FormatException($"Invalid {nameof(TimeSpan)}.");
    }
}
