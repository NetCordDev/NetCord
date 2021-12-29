using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NetCord.Commands
{
    public class CommandServiceOptions : CommandServiceOptions<CommandContext>
    {
    }

    public class CommandServiceOptions<TContext> where TContext : ICommandContext
    {
        public delegate Task<dynamic> TypeReader(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options);

        public Dictionary<Type, TypeReader> TypeReaders { get; } = new()
        #region TypeReaders
        {
            // text types
            {
                typeof(string),
                (input, _, _, _) => Task.FromResult((object)input)
            },
            {
                typeof(char),
                (input, _, _, _) => Task.FromResult(input.Length == 1 ? (object)input[0] : throw new ArgumentParseException("String must be exactly one character long."))
            },
            // integral numeric types
            {
                typeof(sbyte),
                (input, _, _, _) => Task.FromResult((object)sbyte.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(byte),
                (input, _, _, _) => Task.FromResult((object)byte.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(short),
                (input, _, _, _) => Task.FromResult((object)short.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(ushort),
                (input, _, _, _) => Task.FromResult((object)ushort.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(int),
                (input, _, _, _) => Task.FromResult((object)int.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(uint),
                (input, _, _, _) => Task.FromResult((object)uint.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(long),
                (input, _, _, _) => Task.FromResult((object)long.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(nint),
                (input, _, _, _) => Task.FromResult((object)nint.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(nuint),
                (input, _, _, _) => Task.FromResult((object)nuint.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(System.Numerics.BigInteger),
                (input, _, _, _) => Task.FromResult((object)System.Numerics.BigInteger.Parse(input, CultureInfo.InvariantCulture))
            },
            // floating-point numeric types
            {
                typeof(Half),
                (input, _, _, _) => Task.FromResult((object)Half.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(float),
                (input, _, _, _) => Task.FromResult((object)float.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(double),
                (input, _, _, _) => Task.FromResult((object)double.Parse(input, CultureInfo.InvariantCulture))
            },
            {
                typeof(decimal),
                (input, _, _, _) => Task.FromResult((object)decimal.Parse(input, CultureInfo.InvariantCulture))
            },
            // other types
            {
                typeof(bool),
                (input, _, _, _) => Task.FromResult((object)bool.Parse(input))
            },
            {
                typeof(TimeSpan),
                (input, _, _, options) =>
                {
                    RegexOptions regexOptions = options.IgnoreCase ? RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.IgnoreCase
                                                                   : RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft;
                    // Regex blocks minus values
                    var timeSpan = Regex.Match(input, @"^((?<y>\d+)y)?((?<d>\d+)d)?((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?$", regexOptions);
                    if (timeSpan.Success)
                    {
                        var y = timeSpan.Groups["y"];
                        var d = timeSpan.Groups["d"];
                        var h = timeSpan.Groups["h"];
                        var m = timeSpan.Groups["m"];
                        var s = timeSpan.Groups["s"];
                        int days = checked((y.Success ? int.Parse(y.Value) * 365 : 0) + (d.Success ? int.Parse(d.Value) : 0));
                        return Task.FromResult((object)new TimeSpan(days, h.Success ? int.Parse(h.Value) : 0, m.Success ? int.Parse(m.Value) : 0, s.Success ? int.Parse(s.Value) : 0));
                    }
                    else
                        throw new ArgumentParseException("Invalid TimeSpan");
                }
            },
            {
                typeof(TimeOnly),
                (input, _, _, _) => Task.FromResult((object)TimeOnly.Parse(input))
            },
            {
                typeof(DateOnly),
                (input, _, _, _) => Task.FromResult((object)DateOnly.Parse(input))
            },
            {
                typeof(DateTime),
                (input, _, _, _) => Task.FromResult((object)DateTime.Parse(input))
            },
            {
                typeof(DateTimeOffset),
                (input, _, _, _) => Task.FromResult((object)DateTimeOffset.Parse(input))
            },
            {
                typeof(Uri),
                (input, _, _, _) => Task.FromResult((object)new Uri(input))
            },
            {
                typeof(DiscordId),
                (input, _, _, _) => Task.FromResult((object)DiscordId.Parse(input))
            },
            {
                typeof(GuildUser),
                (input, context, _, _) =>
                {
                    var guild = context.Guild;
                    if (guild == null)
                        goto exception;
                    IReadOnlyDictionary<DiscordId, GuildUser> users = guild.Users;
                    // by id
                    if (DiscordId.TryParse(input, out DiscordId? id))
                    {
                        if (users.TryGetValue(id, out var user))
                            return Task.FromResult((object)user);
                    }
                    else
                    {
                        var span = input.AsSpan();
                        // by mention
                        if (MentionUtils.TryParseUser(span, out id))
                        {
                            if (users.TryGetValue(id, out var user))
                                return Task.FromResult((object)user);
                            goto exception;
                        }

                        // by name and tag
                        if (span.Length is >= 7 and <= 37 && span[^5] == '#')
                        {
                            var username = span[..^5].ToString();
                            if (ushort.TryParse(span[^4..], out var discriminator))
                            {
                                var user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                                if (user != null)
                                    return Task.FromResult((object)user);
                            }
                            goto exception;
                        }
                    }

                    // by name or nickname
                    {
                        var len = input.Length;
                        if (len <= 32)
                        {
                            var selectedUsers = len >= 2
                                ? users.Values.Where(u => u.Username == input || u.Nickname == input)
                                : users.Values.Where(u => u.Nickname == input);
                            var count = selectedUsers.Count();
                            if (count == 1)
                                return Task.FromResult((object)selectedUsers.First());
                            else if (count > 1)
                                throw new ArgumentParseException("Too many users found");
                        }
                    }
                    exception:
                    throw new ArgumentParseException("The user was not found");
                }
            },
            {
                typeof(UserId),
                (input, context, _, _) =>
                {
                    var channel = context.Message.Channel;
                    if (context.Guild != null)
                    {
                        IReadOnlyDictionary<DiscordId, GuildUser> users = context.Guild.Users;
                        // by id
                        if (DiscordId.TryParse(input, out DiscordId? id))
                        {
                            users.TryGetValue(id, out var user);
                            return Task.FromResult((object)new UserId(id, user));
                        }

                        var span = input.AsSpan();

                        // by mention
                        if (MentionUtils.TryParseUser(span, out id))
                        {
                            users.TryGetValue(id, out var user);
                            return Task.FromResult((object)new UserId(id, user));
                        }

                        // by name and tag
                        if (span.Length is >= 7 and <= 37 && span[^5] == '#')
                        {
                            var username = span[..^5].ToString();
                            if (ushort.TryParse(span[^4..], out var discriminator))
                            {
                                GuildUser? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                                if (user != null)
                                    return Task.FromResult((object)new UserId(user.Id, user));
                            }
                        }
                        // by name or nickname
                        else
                        {
                            var len = input.Length;
                            if (len <= 32)
                            {
                                var selectedUsers = len >= 2
                                    ? users.Values.Where(u => u.Username == input || u.Nickname == input)
                                    : users.Values.Where(u => u.Nickname == input);
                                var count = selectedUsers.Count();
                                if (count == 1)
                                    return Task.FromResult((object)selectedUsers.First());
                                else if (count > 1)
                                    throw new ArgumentParseException("Too many users found");
                            }
                        }
                    }
                    else if (context.Message.Channel is DMChannel dm)
                    {
                        IReadOnlyDictionary<DiscordId, User> users = dm.Users;
                        // by id
                        if (DiscordId.TryParse(input, out DiscordId? id))
                        {
                            users.TryGetValue(id, out var user);
                            return Task.FromResult((object)new UserId(id, user));
                        }

                        var span = input.AsSpan();

                        // by mention
                        if (MentionUtils.TryParseUser(span, out id))
                        {
                            users.TryGetValue(id, out var user);
                            return Task.FromResult((object)new UserId(id, user));
                        }

                        // by name and tag
                        if (span.Length is >= 7 and <= 37 && span[^5] == '#')
                        {
                            var username = span[..^5].ToString();
                            if (ushort.TryParse(span[^4..], out var discriminator))
                            {
                                User? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                                if (user != null)
                                    return Task.FromResult((object)new UserId(user.Id, user));
                            }
                        }
                        // by name
                        else
                        {
                            if (input.Length is <= 32 and >= 2)
                            {
                                var selectedUsers = users.Values.Select(u => u.Username == input);
                                var count = selectedUsers.Count();
                                if (count == 1)
                                    return Task.FromResult((object)selectedUsers.First());
                                else if (count > 1)
                                    throw new ArgumentParseException("Too many users found");
                            }
                        }
                    }



                    throw new ArgumentParseException("The user was not found");
                }
            },
            {
                typeof(Timestamp),
                (input, _, _, _) => Task.FromResult((object)Timestamp.Parse(input))
            },
            {
                typeof(CodeBlock),
                (input, _, _, _) => Task.FromResult((object)CodeBlock.Parse(input))
            }
        };
        #endregion

        public TypeReader EnumTypeReader { get; init; } = (input, context, parameter, options) =>
        {
            var type = parameter.Type;
            if (parameter.Attributes.TryGetValue(typeof(AllowByValueAttribute).GUID, out _) || type.IsDefined(typeof(AllowByValueAttribute))) // by value or by name
            {
                if (Enum.TryParse(type, input, options.IgnoreCase, out var value) && Enum.IsDefined(type, value!))
                    return Task.FromResult(value!);
            }
            else // by name
            {
                if (((uint)input[0] - '0') > 9 && Enum.TryParse(type, input, options.IgnoreCase, out var value))
                    return Task.FromResult(value!);
            }

            throw new ArgumentParseException($"Invalid {type.Name}");
        };

        /// <summary>
        /// Default = ' '
        /// </summary>
        public char ParamSeparator { get; init; } = ' ';

        /// <summary>
        /// Default = <see cref="true"/>
        /// </summary>
        public bool IgnoreCase { get; init; } = true;
    }
}
