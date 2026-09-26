using System.Buffers;
using System.Collections.Immutable;
using System.Globalization;

namespace NetCord.Services.Commands;

public record CommandServiceConfiguration<TContext> where TContext : ICommandContext
{
    public static CommandServiceConfiguration<TContext> Default { get; } = new();

    private CommandServiceConfiguration()
    {
        char[] defaultSeparators = [' ', '\n'];
        _parameterSeparators = new(defaultSeparators, SearchValues.Create(defaultSeparators));
    }

    public ImmutableDictionary<Type, CommandTypeReader<TContext>> TypeReaders { get; init; } = new Dictionary<Type, CommandTypeReader<TContext>>()
    {
    #region TypeReaders
        // text types
        { typeof(string), new TypeReaders.StringTypeReader<TContext>() },
        { typeof(ReadOnlyMemory<char>), new TypeReaders.ReadOnlyMemoryOfCharTypeReader<TContext>() },
        { typeof(char), new TypeReaders.CharTypeReader<TContext>() },
        // integral numeric types
        { typeof(sbyte), new TypeReaders.SByteTypeReader<TContext>() },
        { typeof(byte), new TypeReaders.ByteTypeReader<TContext>() },
        { typeof(short), new TypeReaders.Int16TypeReader<TContext>() },
        { typeof(ushort), new TypeReaders.UInt16TypeReader<TContext>() },
        { typeof(int), new TypeReaders.Int32TypeReader<TContext>() },
        { typeof(uint), new TypeReaders.UInt32TypeReader<TContext>() },
        { typeof(long), new TypeReaders.Int64TypeReader<TContext>() },
        { typeof(Int128), new TypeReaders.Int128TypeReader<TContext>() },
        { typeof(ulong), new TypeReaders.UInt64TypeReader<TContext>() },
        { typeof(UInt128), new TypeReaders.UInt128TypeReader<TContext>() },
        { typeof(nint), new TypeReaders.IntPtrTypeReader<TContext>() },
        { typeof(nuint), new TypeReaders.UIntPtrTypeReader<TContext>() },
        { typeof(System.Numerics.BigInteger), new TypeReaders.BigIntegerTypeReader<TContext>() },
        // floating-point numeric types
        { typeof(Half), new TypeReaders.HalfTypeReader<TContext>() },
        { typeof(float), new TypeReaders.SingleTypeReader<TContext>() },
        { typeof(double), new TypeReaders.DoubleTypeReader<TContext>() },
        { typeof(decimal), new TypeReaders.DecimalTypeReader<TContext>() },
        // channels
        { typeof(CategoryGuildChannel), new TypeReaders.CategoryGuildChannelTypeReader<TContext>() },
        { typeof(Channel), new TypeReaders.ChannelTypeReader<TContext>() },
        { typeof(IGuildChannel), new TypeReaders.IGuildChannelTypeReader<TContext>() },
        { typeof(INamedChannel), new TypeReaders.INamedChannelTypeReader<TContext>() },

        { typeof(DMChannel), new TypeReaders.DMChannelTypeReader<TContext>() },
        { typeof(GroupDMChannel), new TypeReaders.GroupDMChannelTypeReader<TContext>() },
        { typeof(TextChannel), new TypeReaders.TextChannelTypeReader<TContext>() },

        { typeof(AnnouncementGuildChannel), new TypeReaders.AnnouncementGuildChannelTypeReader<TContext>() },
        { typeof(DirectoryGuildChannel), new TypeReaders.DirectoryGuildChannelTypeReader<TContext>() },
        { typeof(ForumGuildChannel), new TypeReaders.ForumGuildChannelTypeReader<TContext>() },
        { typeof(MediaForumGuildChannel), new TypeReaders.MediaForumGuildChannelTypeReader<TContext>() },
        { typeof(TextGuildChannel), new TypeReaders.TextGuildChannelTypeReader<TContext>() },

        { typeof(AnnouncementGuildThread), new TypeReaders.AnnouncementGuildThreadTypeReader<TContext>() },
        { typeof(GuildThread), new TypeReaders.GuildThreadTypeReader<TContext>() },
        { typeof(PrivateGuildThread), new TypeReaders.PrivateGuildThreadTypeReader<TContext>() },
        { typeof(PublicGuildThread), new TypeReaders.PublicGuildThreadTypeReader<TContext>() },

        { typeof(IVoiceGuildChannel), new TypeReaders.IVoiceGuildChannelTypeReader<TContext>() },
        { typeof(StageGuildChannel), new TypeReaders.StageGuildChannelTypeReader<TContext>() },
        { typeof(VoiceGuildChannel), new TypeReaders.VoiceGuildChannelTypeReader<TContext>() },
        // other types
        { typeof(bool), new TypeReaders.BooleanTypeReader<TContext>() },
        { typeof(TimeSpan), new TypeReaders.TimeSpanTypeReader<TContext>() },
        { typeof(TimeOnly), new TypeReaders.TimeOnlyTypeReader<TContext>() },
        { typeof(DateOnly), new TypeReaders.DateOnlyTypeReader<TContext>() },
        { typeof(DateTime), new TypeReaders.DateTimeTypeReader<TContext>() },
        { typeof(DateTimeOffset), new TypeReaders.DateTimeOffsetTypeReader<TContext>() },
        { typeof(Uri), new TypeReaders.UriTypeReader<TContext>() },
        { typeof(User), new TypeReaders.UserTypeReader<TContext>() },
        { typeof(GuildUser), new TypeReaders.GuildUserTypeReader<TContext>() },
        { typeof(UserId), new TypeReaders.UserIdTypeReader<TContext>() },
        { typeof(Timestamp), new TypeReaders.TimestampTypeReader<TContext>() },
        { typeof(CodeBlock), new TypeReaders.CodeBlockTypeReader<TContext>() }
    #endregion
    }.ToImmutableDictionary();

    public CommandTypeReader<TContext> EnumTypeReader { get; init; } = new TypeReaders.EnumTypeReader<TContext>();

    /// <summary>
    /// Defaults to <c>[' ', '\n']</c>.
    /// </summary>
    public IEnumerable<char> ParameterSeparators
    {
        get
        {
            return _parameterSeparators.Item1;
        }
        init
        {
            _parameterSeparators = new(value, SearchValues.Create(value.ToArray()));
        }
    }

    internal SearchValues<char> ParameterSeparatorsSearchValues => _parameterSeparators.Item2;

    private readonly Tuple<IEnumerable<char>, SearchValues<char>> _parameterSeparators;

    /// <summary>
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool IgnoreCase { get; init; } = true;

    internal ReadOnlyMemoryCharComparer Comparer => IgnoreCase ? ReadOnlyMemoryCharComparer.InvariantCultureIgnoreCase
                                                               : ReadOnlyMemoryCharComparer.InvariantCulture;

    public CultureInfo CultureInfo { get; init; } = CultureInfo.InvariantCulture;

    public IResultResolverProvider<TContext> ResultResolverProvider { get; init; } = CommandResultResolverProvider<TContext>.Instance;

    public IServiceResolverProvider ServiceResolverProvider { get; init; } = Services.ServiceResolverProvider.Instance;
}
