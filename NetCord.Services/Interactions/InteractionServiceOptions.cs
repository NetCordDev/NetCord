using System.Globalization;

namespace NetCord.Services.Interactions;

public class InteractionServiceOptions<TContext> where TContext : InteractionContext
{
    public Dictionary<Type, InteractionTypeReader<TContext>> TypeReaders { get; } = new()
    #region TypeReaders
    {
        // text types
        {
            typeof(string),
            new TypeReaders.StringTypeReader<TContext>()
        },
        {
            typeof(char),
            new TypeReaders.CharTypeReader<TContext>()
        },
        // integral numeric types
        {
            typeof(sbyte),
            new TypeReaders.SByteTypeReader<TContext>()
        },
        {
            typeof(byte),
            new TypeReaders.ByteTypeReader<TContext>()
        },
        {
            typeof(short),
            new TypeReaders.Int16TypeReader<TContext>()
        },
        {
            typeof(ushort),
            new TypeReaders.UInt16TypeReader<TContext>()
        },
        {
            typeof(int),
            new TypeReaders.Int32TypeReader<TContext>()
        },
        {
            typeof(uint),
            new TypeReaders.UInt32TypeReader<TContext>()
        },
        {
            typeof(long),
            new TypeReaders.Int64TypeReader<TContext>()
        },
        {
            typeof(ulong),
            new TypeReaders.UInt64TypeReader<TContext>()
        },
        {
            typeof(nint),
            new TypeReaders.IntPtrTypeReader<TContext>()
        },
        {
            typeof(nuint),
            new TypeReaders.UIntPtrTypeReader<TContext>()
        },
        {
            typeof(System.Numerics.BigInteger),
            new TypeReaders.BigIntegerTypeReader<TContext>()
        },
        // floating-point numeric types
        {
            typeof(Half),
            new TypeReaders.HalfTypeReader<TContext>()
        },
        {
            typeof(float),
            new TypeReaders.SingleTypeReader<TContext>()
        },
        {
            typeof(double),
            new TypeReaders.DoubleTypeReader<TContext>()
        },
        {
            typeof(decimal),
            new TypeReaders.DecimalTypeReader<TContext>()
        },
        // other types
        {
            typeof(bool),
            new TypeReaders.BooleanTypeReader<TContext>()
        },
        {
            typeof(TimeSpan),
            new TypeReaders.TimeSpanTypeReader<TContext>()
        },
        {
            typeof(TimeOnly),
            new TypeReaders.TimeOnlyTypeReader<TContext>()
        },
        {
            typeof(DateOnly),
            new TypeReaders.DateOnlyTypeReader<TContext>()
        },
        {
            typeof(DateTime),
            new TypeReaders.DateTimeTypeReader<TContext>()
        },
        {
            typeof(DateTimeOffset),
            new TypeReaders.DateTimeOffsetTypeReader<TContext>()
        },
        {
            typeof(Uri),
            new TypeReaders.UriTypeReader<TContext>()
        },
        {
            typeof(GuildUser),
            new TypeReaders.GuildUserTypeReader<TContext>()
        },
        {
            typeof(UserId),
            new TypeReaders.UserIdTypeReader<TContext>()
        },
        {
            typeof(Timestamp),
            new TypeReaders.TimestampTypeReader<TContext>()
        },
        {
            typeof(CodeBlock),
            new TypeReaders.CodeBlockTypeReader<TContext>()
        }
    };
    #endregion

    public InteractionTypeReader<TContext> EnumTypeReader { get; init; } = new TypeReaders.EnumTypeReader<TContext>();

    /// <summary>
    /// Default = <see langword="false"/>
    /// </summary>
    public bool IgnoreCase { get; init; }

    public char ParamSeparator { get; init; } = ':';

    public CultureInfo CultureInfo { get; init; } = CultureInfo.InvariantCulture;
}
