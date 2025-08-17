using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NetCord;

/// <summary>
/// Provides utilities for creating, parsing and extracting values from snowflake identifiers.
/// </summary>
public static class Snowflake
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong CreateCore(long timestamp, bool validate)
    {
        var discordTimestamp = (ulong)(timestamp - Discord.Epoch);

        if (validate && (discordTimestamp & ~((1uL << 42) - 1)) != 0)
            ThrowArgumentOutOfRange(nameof(timestamp));

        return discordTimestamp << 22;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong CreateCore(long timestamp, byte internalWorkerId, byte internalProcessId, ushort increment, bool validate)
    {
        var discordTimestamp = (ulong)(timestamp - Discord.Epoch);

        string? parameterName;
        if (validate)
        {
            if ((discordTimestamp & ~((1uL << 42) - 1)) != 0)
            {
                parameterName = nameof(timestamp);
                goto Throw;
            }

            if ((internalWorkerId & ~((1 << 5) - 1)) != 0)
            {
                parameterName = nameof(internalWorkerId);
                goto Throw;
            }

            if ((internalProcessId & ~((1 << 5) - 1)) != 0)
            {
                parameterName = nameof(internalProcessId);
                goto Throw;
            }

            if ((increment & ~((1 << 12) - 1)) != 0)
            {
                parameterName = nameof(increment);
                goto Throw;
            }
        }

        var c = discordTimestamp << 22;

        var w = (ulong)(internalWorkerId << 17);

        var p = (ulong)(internalProcessId << 12);

        var i = (ulong)increment;

        return c | w | p | i;

        Throw:
        ThrowArgumentOutOfRange(parameterName);
        return default;
    }

    [DoesNotReturn]
    private static void ThrowArgumentOutOfRange(string parameterName)
    {
        throw new ArgumentOutOfRangeException(parameterName);
    }

    /// <summary>
    /// Creates a snowflake. Skips validation.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created, in Unix time milliseconds.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong CreateUnsafe(long timestamp)
    {
        return CreateCore(timestamp, false);
    }

    /// <summary>
    /// Creates a snowflake. Skips validation.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created, in Unix time milliseconds.</param>
    /// <param name="internalWorkerId">The ID of the internal worker that created the snowflake. Must be in range 0-31.</param>
    /// <param name="internalProcessId">The ID of the internal process that created the snowflake. Must be in range 0-31.</param>
    /// <param name="increment">The increment value for the snowflake. Must be in range 0-4095.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong CreateUnsafe(long timestamp, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        return CreateCore(timestamp, internalWorkerId, internalProcessId, increment, false);
    }

    /// <summary>
    /// Creates a snowflake.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created, in Unix time milliseconds.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong Create(long timestamp)
    {
        return CreateCore(timestamp, true);
    }

    /// <summary>
    /// Creates a snowflake.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created, in Unix time milliseconds.</param>
    /// <param name="internalWorkerId">The ID of the internal worker that created the snowflake. Must be in range 0-31.</param>
    /// <param name="internalProcessId">The ID of the internal process that created the snowflake. Must be in range 0-31.</param>
    /// <param name="increment">The increment value for the snowflake. Must be in range 0-4095.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong Create(long timestamp, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        return CreateCore(timestamp, internalWorkerId, internalProcessId, increment, true);
    }

    /// <summary>
    /// Creates a snowflake. Skips validation.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created. Sub-millisecond precision is truncated.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong CreateUnsafe(DateTimeOffset timestamp)
    {
        return CreateCore(timestamp.ToUnixTimeMilliseconds(), false);
    }

    /// <summary>
    /// Creates a snowflake. Skips validation.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created. Sub-millisecond precision is truncated.</param>
    /// <param name="internalWorkerId">The ID of the internal worker that created the snowflake. Must be in range 0-31.</param>
    /// <param name="internalProcessId">The ID of the internal process that created the snowflake. Must be in range 0-31.</param>
    /// <param name="increment">The increment value for the snowflake. Must be in range 0-4095.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong CreateUnsafe(DateTimeOffset timestamp, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        return CreateCore(timestamp.ToUnixTimeMilliseconds(), internalWorkerId, internalProcessId, increment, false);
    }

    /// <summary>
    /// Creates a snowflake.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created. Sub-millisecond precision is truncated.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong Create(DateTimeOffset timestamp)
    {
        return CreateCore(timestamp.ToUnixTimeMilliseconds(), true);
    }

    /// <summary>
    /// Creates a snowflake.
    /// </summary>
    /// <param name="timestamp">The timestamp at which the snowflake was created. Sub-millisecond precision is truncated.</param>
    /// <param name="internalWorkerId">The ID of the internal worker that created the snowflake. Must be in range 0-31.</param>
    /// <param name="internalProcessId">The ID of the internal process that created the snowflake. Must be in range 0-31.</param>
    /// <param name="increment">The increment value for the snowflake. Must be in range 0-4095.</param>
    /// <returns>The created snowflake.</returns>
    public static ulong Create(DateTimeOffset timestamp, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        return CreateCore(timestamp.ToUnixTimeMilliseconds(), internalWorkerId, internalProcessId, increment, true);
    }

    /// <summary>
    /// Gets the timestamp at which the snowflake was created.
    /// </summary>
    /// <param name="id">The snowflake identifier.</param>
    /// <returns>The timestamp at which the snowflake was created.</returns>
    public static DateTimeOffset Timestamp(ulong id) => DateTimeOffset.FromUnixTimeMilliseconds((long)((id >> 22) + Discord.Epoch));

    /// <summary>
    /// Gets the ID of the internal worker that created the snowflake.
    /// </summary>
    /// <param name="id">The snowflake identifier.</param>
    /// <returns>The ID of the internal worker that created the snowflake.</returns>
    public static byte InternalWorkerId(ulong id) => (byte)((id & 0x3E0000) >> 17);

    /// <summary>
    /// Gets the ID of the internal process that created the snowflake.
    /// </summary>
    /// <param name="id">The snowflake identifier.</param>
    /// <returns>The ID of the internal process that created the snowflake.</returns>
    public static byte InternalProcessId(ulong id) => (byte)((id & 0x1F000) >> 12);

    /// <summary>
    /// Gets the increment value for the snowflake.
    /// </summary>
    /// <param name="id">The snowflake identifier.</param>
    /// <returns>The increment value for the snowflake.</returns>
    public static ushort Increment(ulong id) => (ushort)(id & 0xFFF);

    /// <summary>
    /// Gets the ID of the shard that the guild belongs to.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="shardCount">The total number of shards.</param>
    /// <returns>The ID of the shard that the guild belongs to.</returns>
    public static int ShardId(ulong guildId, int shardCount) => (int)((guildId >> 22) % (ulong)shardCount);

    /// <summary>
    /// Tries to parse a snowflake from a span of characters.
    /// </summary>
    /// <param name="value">The span of characters to parse.</param>
    /// <param name="id">The resulting snowflake identifier.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> was parsed successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<char> value, out ulong id) => ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out id);

    /// <summary>
    /// Parses a snowflake from a span of characters.
    /// </summary>
    /// <param name="value">The span of characters to parse.</param>
    /// <returns>The resulting snowflake identifier.</returns>
    public static ulong Parse(ReadOnlySpan<char> value) => ulong.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);

    /// <summary>
    /// Tries to parse a snowflake from a span of UTF-8 characters.
    /// </summary>
    /// <param name="value">The span of UTF-8 characters to parse.</param>
    /// <param name="id">The resulting snowflake identifier.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> was parsed successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(ReadOnlySpan<byte> value, out ulong id) => Utf8Parser.TryParse(value, out id, out int bytesConsumed) && bytesConsumed == value.Length;

    /// <summary>
    /// Parses a snowflake from a span of UTF-8 characters.
    /// </summary>
    /// <param name="value">The span of UTF-8 characters to parse.</param>
    /// <returns>The resulting snowflake identifier.</returns>
    public static ulong Parse(ReadOnlySpan<byte> value)
    {
        if (TryParse(value, out var id))
            return id;

        ThrowFormatException();
        return default;
    }

    [DoesNotReturn]
    private static void ThrowFormatException()
    {
        throw new FormatException("The input is not in a correct format.");
    }
}
