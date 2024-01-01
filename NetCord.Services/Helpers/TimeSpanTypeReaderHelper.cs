using System.Globalization;
using System.Text.RegularExpressions;

namespace NetCord.Services.Helpers;

internal static partial class TimeSpanTypeReaderHelper
{
    public static TypeReaderResult Read(string input, bool ignoreCase, CultureInfo cultureInfo, string parameterName)
    {
        var timeSpan = GetRegex(ignoreCase).Match(input.ToString());
        if (timeSpan.Success)
        {
            var groups = timeSpan.Groups;

            var y = groups["y"];
            var d = groups["d"];
            var h = groups["h"];
            var m = groups["m"];
            var s = groups["s"];

            int days, hours, minutes, seconds;
            checked
            {
                if (y.Success)
                {
                    if (int.TryParse(y.Value, NumberStyles.None, cultureInfo, out var years))
                        days = years * 365;
                    else
                        goto Fail;
                }
                else
                    days = 0;

                if (d.Success)
                {
                    if (int.TryParse(d.Value, NumberStyles.None, cultureInfo, out var additionalDays))
                        days += additionalDays;
                    else
                        goto Fail;
                }

                if (h.Success)
                {
                    if (!int.TryParse(h.Value, NumberStyles.None, cultureInfo, out hours))
                        goto Fail;
                }
                else
                    hours = 0;

                if (m.Success)
                {
                    if (!int.TryParse(m.Value, NumberStyles.None, cultureInfo, out minutes))
                        goto Fail;
                }
                else
                    minutes = 0;

                if (s.Success)
                {
                    if (!int.TryParse(s.Value, NumberStyles.None, cultureInfo, out seconds))
                        goto Fail;
                }
                else
                    seconds = 0;
            }

            return TypeReaderResult.Success(new TimeSpan(days, hours, minutes, seconds));
        }

        Fail:
        return TypeReaderResult.ParseFail(parameterName);
    }

    private static Regex GetRegex(bool ignoreCase) => ignoreCase ? GetIgnoreCaseRegex() : GetRegex();

    [GeneratedRegex(@"^((?<y>\d+)y)?((?<d>\d+)d)?((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex GetIgnoreCaseRegex();

    [GeneratedRegex(@"^((?<y>\d+)y)?((?<d>\d+)d)?((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant)]
    private static partial Regex GetRegex();
}
