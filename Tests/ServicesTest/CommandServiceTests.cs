using System.Numerics;
using System.Runtime.InteropServices;

using NetCord;
using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Services.Commands;

namespace ServicesTest;

[TestClass]
public class CommandServiceTests
{
    private readonly GatewayClient _client = new(new BotToken("ODAzMzc3MjcwODc4MTA5NzI2.GAVT0D.DDNY-77JFnrMDZxSSwlq3WdlZH-3grIPBPKrSA"));

    private Message CreateMessage(string content)
    {
        JsonMessage jsonModel = new()
        {
            Author = new(),
            MentionedUsers = [],
            Attachments = [],
            Embeds = [],
            Content = content,
        };

        return new(jsonModel, null, null, _client.Rest);
    }

    [TestMethod]
    public async ValueTask Basic()
    {
        await ExecuteMultipleAsync(
            ["test", "test-test"],
            ResultHandler.Success(),
            () => { }).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterMatching()
    {
        await ExecuteMultipleAsync(
            [
                "test test",
                "test\ntest",
                "test \ntest",
                "test \n test",
                "test           test",
                "test\n\n\n\n\n\n\n\n\ntest",
                "test\n\n  \n\n    \n\n \n\n\ntest"
            ],
            ResultHandler.DataMatch("test"),
            (string s) =>
            {
                Body.Data(s);
            }).ConfigureAwait(false);
    }

    [TestMethod]
    public async ValueTask ParameterParsing()
    {
        (string, BigInteger?)[] integers = [
            // Basic values
            ("1", 1),
            ("-1", -1),
            ("0", 0),
            
            // Boundary values for common integer types
            ("127", sbyte.MaxValue),
            ("-128", sbyte.MinValue),
            ("255", byte.MaxValue),
            ("0", byte.MinValue),
            ("32767", short.MaxValue),
            ("-32768", short.MinValue),
            ("65535", ushort.MaxValue),
            ("0", ushort.MinValue),
            ("2147483647", int.MaxValue),
            ("-2147483648", int.MinValue),
            ("4294967295", uint.MaxValue),
            ("0", uint.MinValue),
            ("9223372036854775807", long.MaxValue),
            ("-9223372036854775808", long.MinValue),
            ("18446744073709551615", ulong.MaxValue),
            ("0", ulong.MinValue),
            
            // Large BigInteger values
            ("1234567890123456789012345678901234567890", BigInteger.Parse("1234567890123456789012345678901234567890")),
            ("-1234567890123456789012345678901234567890", BigInteger.Parse("-1234567890123456789012345678901234567890")),
            
            // Values near type boundaries that test overflow handling
            ("340282366920938463463374607431768211455", BigInteger.Parse("340282366920938463463374607431768211455")), // UInt128.MaxValue
            ("-170141183460469231731687303715884105728", BigInteger.Parse("-170141183460469231731687303715884105728")), // Int128.MinValue
            
            // Hexadecimal values
            ("0x1A", null),
            ("0xFF", null),
            ("0x7FFFFFFF", null),
            ("-0x80000000", null),
            
            // Binary values
            ("0b1010", null),
            ("0b11111111", null),
            
            // Octal values
            ("0o777", null),
            ("0o123", null),
            
            // Values with leading zeros
            ("000123", 123),
            ("-000456", -456),
            
            // Scientific notation
            ("1e3", null),
            ("1.5e2", null),
            
            // Invalid integer formats
            ("abc", null),
            ("12.34", null),
            ("12.0", null),
            ("1,000", null),
            ("1_000", null),
            ("NaN", null),
            ("Infinity", null),
            ("-Infinity", null),
            ("1.2.3", null),
            ("++1", null),
            ("--1", null),
            ("+-1", null),
            ("-+1", null),
            ("1-", null),
            ("1+", null),
            ("0x", null),
            ("0b", null),
            ("0o", null),
            ("0xG", null),
            ("0b2", null),
            ("0o8", null),
        ];

        await SimpleInteger<sbyte>(integers).ConfigureAwait(false);
        await SimpleInteger<byte>(integers).ConfigureAwait(false);
        await SimpleInteger<short>(integers).ConfigureAwait(false);
        await SimpleInteger<ushort>(integers).ConfigureAwait(false);
        await SimpleInteger<int>(integers).ConfigureAwait(false);
        await SimpleInteger<uint>(integers).ConfigureAwait(false);
        await SimpleInteger<long>(integers).ConfigureAwait(false);
        await SimpleInteger<ulong>(integers).ConfigureAwait(false);
        await SimpleInteger<nint>(integers).ConfigureAwait(false);
        await SimpleInteger<nuint>(integers).ConfigureAwait(false);
        await SimpleInteger<Int128>(integers).ConfigureAwait(false);
        await SimpleInteger<UInt128>(integers).ConfigureAwait(false);
        await SimpleInteger<BigInteger>(integers).ConfigureAwait(false);

        (string, double?)[] floats = [
            // Basic values
            ("1", 1),
            ("1.0", 1),
            ("0", 0),
            ("0.0", 0),
            ("-1", -1),
            ("-1.0", -1),
            ("0.5", 0.5),
            ("-0.5", -0.5),
            ("1.", 1.0),
            (".5", 0.5),
            
            // Decimal values
            ("3.14159", 3.14159),
            ("-2.71828", -2.71828),
            ("0.123456789", 0.123456789),
            
            // Scientific notation
            ("1e3", 1000.0),
            ("1E3", 1000.0),
            ("1.5e2", 150.0),
            ("1.5E-2", 0.015),
            ("-1.23e-4", -0.000123),
            ("6.022e23", 6.022e23),
            ("2.5e+3", 2500.0),
            ("1e0", 1.0),
            ("1E+0", 1.0),
            ("1E-0", 1.0),
            ("3.14159e1", 31.4159),
            ("-2.71828E2", -271.828),
            ("1.0e-10", 1.0e-10),
            ("9.999e99", 9.999e99),
            ("-1.234e-50", -1.234e-50),
            
            // Special values
            ("Infinity", double.PositiveInfinity),
            ("-Infinity", double.NegativeInfinity),
            ("NaN", double.NaN),
            
            // Values with leading/trailing whitespace
            (" 1.0 ", 1),
            
            // Invalid float formats
            ("abc", null),
            ("1.2.3", null),
            ("1,000.5", null),
            ("1_000.5", null),
            ("++1.0", null),
            ("--1.0", null),
            ("+-1.0", null),
            ("-+1.0", null),
            ("1.0-", null),
            ("1.0+", null),
            (".", null),
            ("..", null),
            ("e", null),
            ("1e", null),
            ("1e+", null),
            ("1e-", null),
            ("1ee2", null),
            ("1e2.5", null), // Exponent must be integer
            ("1e+2.5", null), // Exponent must be integer
            ("1e-2.5", null), // Exponent must be integer
        ];

        await SimpleFloatingPointIeee754<Half>(floats).ConfigureAwait(false);
        await SimpleFloatingPointIeee754<float>(floats).ConfigureAwait(false);
        await SimpleFloatingPointIeee754<double>(floats).ConfigureAwait(false);

        (string, decimal?)[] decimals = [
            // Basic values
            ("1", 1m),
            ("1.0", 1.0m),
            ("0", 0m),
            ("0.0", 0.0m),
            ("-1", -1m),
            ("-1.0", -1.0m),
            ("0.5", 0.5m),
            ("-0.5", -0.5m),
            ("1.", 1.0m),
            (".5", 0.5m),
            
            // High precision decimal values
            ("3.1415926535897932384626433832795", 3.1415926535897932384626433832795m),
            ("-2.7182818284590452353602874713527", -2.7182818284590452353602874713527m),
            ("0.123456789012345678901234567890", 0.123456789012345678901234567890m),
            ("1.00000000000000000000000000001", 1.00000000000000000000000000001m),
            
            // Boundary values for decimal
            ("79228162514264337593543950335", decimal.MaxValue),
            ("-79228162514264337593543950335", decimal.MinValue),
            ("0.0000000000000000000000000001", 0.0000000000000000000000000001m),
            
            // Money-like values
            ("19.99", 19.99m),
            ("1000000.01", 1000000.01m),
            ("-0.01", -0.01m),
            
            // Values with many decimal places
            ("1.0000000000000000000000000000", 1.0000000000000000000000000000m),
            ("123.4567890123456789012345678", 123.4567890123456789012345678m),
            
            // Scientific notation
            ("1e3", null),
            ("1.5e2", null),
            ("1E-5", null),
            
            // Invalid decimal formats
            ("abc", null),
            ("1.2.3", null),
            ("1,000.5", null),
            ("1_000.5", null),
            ("NaN", null),
            ("Infinity", null),
            ("-Infinity", null),
            ("++1.0", null),
            ("--1.0", null),
            ("+-1.0", null),
            ("-+1.0", null),
            ("1.0-", null),
            ("1.0+", null),
            (".", null),
            ("..", null),
            
            // Values that exceed decimal precision
            ("79228162514264337593543950336", null), // Beyond decimal.MaxValue
            ("-79228162514264337593543950336", null), // Beyond decimal.MinValue
        ];

        await SimpleDecimal(decimals).ConfigureAwait(false);

        ValueTask SimpleInteger<T>((string ValueString, BigInteger? Value)[] values) where T : struct, IBinaryInteger<T>
        {
            return SimpleNumber<T, BigInteger>(values);
        }

        ValueTask SimpleFloatingPointIeee754<T>((string ValueString, double? Value)[] values) where T : struct, IFloatingPointIeee754<T>
        {
            return SimpleNumber<T, double>(values);
        }

        ValueTask SimpleDecimal((string ValueString, decimal? Value)[] values)
        {
            return SimpleNumber<decimal, decimal>(values);
        }

        async ValueTask SimpleNumber<T, TGenericStorage>((string ValueString, TGenericStorage? Value)[] values) where T : struct, INumberBase<T> where TGenericStorage : struct, INumberBase<TGenericStorage>
        {
            foreach (var (valueString, value) in values)
            {
                T? casted;

                if (value.HasValue)
                {
                    casted = T.CreateSaturating(value.GetValueOrDefault());
                    var again = TGenericStorage.CreateChecked(casted.GetValueOrDefault());

                    if (value != again)
                        continue;
                }
                else
                    casted = null;

                await ExecuteAsync(
                        $"test {valueString}",
                        casted.HasValue ? ResultHandler.DataMatch(casted.GetValueOrDefault()) : ResultHandler.ParseFail(),
                        (T number) => Body.Data(number)).ConfigureAwait(false);
            }
        }
    }

    private enum TestEnum { A, B }

    private enum SmallTestEnum : sbyte { A, B }

    private enum BigTestEnum : long { A, B }

    [TestMethod]
    public async ValueTask OptionalParameterValues()
    {
        // Only optional attribute value types
        await OnlyOptionalAttributeValueType<sbyte>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<byte>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<short>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<ushort>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<int>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<uint>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<long>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<ulong>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<nint>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<nuint>().ConfigureAwait(false);

        await OnlyOptionalAttributeValueType<Int128>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<UInt128>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<BigInteger>().ConfigureAwait(false);

        await OnlyOptionalAttributeValueType<Half>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<float>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<double>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<decimal>().ConfigureAwait(false);

        await OnlyOptionalAttributeValueType<TestEnum>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<SmallTestEnum>().ConfigureAwait(false);
        await OnlyOptionalAttributeValueType<BigTestEnum>().ConfigureAwait(false);

        await OnlyOptionalAttributeValueType<ReadOnlyMemory<char>>().ConfigureAwait(false);

        // Only optional attribute reference types
        await OnlyOptionalAttributeReferenceType<string>().ConfigureAwait(false);
        await OnlyOptionalAttributeReferenceType<User>().ConfigureAwait(false);

        // Optional with custom default value value types
        await OptionalWithCustomDefaultValueValueType((sbyte)10, (sbyte n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((byte)10, (byte n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((short)10, (short n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((ushort)10, (ushort n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType(10, (int n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((uint)10, (uint n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((long)10, (long n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((ulong)10, (ulong n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((nint)10, (nint n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((nuint)10, (nuint n = 10) => Body.Data(n)).ConfigureAwait(false);

        await OptionalWithCustomDefaultValueValueType((float)10, (float n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((double)10, (double n = 10) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType((decimal)10, (decimal n = 10) => Body.Data(n)).ConfigureAwait(false);

        await OptionalWithCustomDefaultValueValueType(TestEnum.B, (TestEnum n = TestEnum.B) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType(SmallTestEnum.B, (SmallTestEnum n = SmallTestEnum.B) => Body.Data(n)).ConfigureAwait(false);
        await OptionalWithCustomDefaultValueValueType(BigTestEnum.B, (BigTestEnum n = BigTestEnum.B) => Body.Data(n)).ConfigureAwait(false);

        // Optional with custom default value reference types
        await OptionalWithCustomDefaultValueReferenceType("foo", (string s = "foo") => Body.Data(s)).ConfigureAwait(false);

        // Optional with default value specific value types edge case (generic proxy causes some value type parameters to have an invalid null default value)
        await OptionalWithDefaultValueEdgeCaseValueType<sbyte>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<byte>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<short>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<ushort>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<int>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<uint>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<long>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<ulong>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<nint>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<nuint>().ConfigureAwait(false);

        await OptionalWithDefaultValueEdgeCaseValueType<float>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<double>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<decimal>().ConfigureAwait(false);

        await OptionalWithDefaultValueEdgeCaseValueType<TestEnum>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<SmallTestEnum>().ConfigureAwait(false);
        await OptionalWithDefaultValueEdgeCaseValueType<BigTestEnum>().ConfigureAwait(false);

        // Optional with default value reference types
        await OptionalWithDefaultValueReferenceType<string>().ConfigureAwait(false);
        await OptionalWithDefaultValueReferenceType<User>().ConfigureAwait(false);

        async ValueTask OnlyOptionalAttributeValueType<T>() where T : struct
        {
            await ExecuteAsync("test", ResultHandler.DataMatch<T>(default), ([Optional] T value) => Body.Data(value)).ConfigureAwait(false);

            await ExecuteAsync("test", ResultHandler.DataMatch<T?>(null), ([Optional] T? value) => Body.Data(value)).ConfigureAwait(false);
        }

        ValueTask OnlyOptionalAttributeReferenceType<T>() where T : class
        {
            return ExecuteAsync("test", ResultHandler.DataMatch<T?>(null), ([Optional] T? value) => Body.Data(value));
        }

        ValueTask OptionalWithCustomDefaultValueValueType<T>(T defaultValue, Delegate handler) where T : struct
        {
            return ExecuteAsync("test", ResultHandler.DataMatch(defaultValue), handler);
        }

        ValueTask OptionalWithCustomDefaultValueReferenceType<T>(T defaultValue, Delegate handler) where T : class
        {
            return ExecuteAsync("test", ResultHandler.DataMatch(defaultValue), handler);
        }

        async ValueTask OptionalWithDefaultValueEdgeCaseValueType<T>() where T : struct
        {
            await ExecuteAsync("test", ResultHandler.DataMatch<T>(default), (T value = default) => Body.Data(value)).ConfigureAwait(false);

            await ExecuteAsync("test", ResultHandler.DataMatch<T?>(null), (T? value = null) => Body.Data(value)).ConfigureAwait(false);
        }

        ValueTask OptionalWithDefaultValueReferenceType<T>() where T : class
        {
            return ExecuteAsync("test", ResultHandler.DataMatch<T?>(null), (T? value = null) => Body.Data(value));
        }
    }

    private async ValueTask ExecuteMultipleAsync(string[] commands, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        foreach (var command in commands)
            await ExecuteAsync(command, resultHandler, handler, services).ConfigureAwait(false);
    }

    private async ValueTask ExecuteAsync(string command, ResultHandler resultHandler, Delegate handler, IServiceProvider? services = null)
    {
        var config = CommandServiceConfiguration<CommandContext>.Default;

        CommandService<CommandContext> service = new(config);

        var commandName = command.Split([.. config.ParameterSeparators])[0];

        service.AddCommand([commandName], handler);

        var message = CreateMessage(command);
        CommandContext context = new(message, _client);
        var result = await service.ExecuteAsync(0, context, services).ConfigureAwait(false);

        try
        {
            resultHandler.Handle(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Assertion failed for command '{command}'.\n{ex.Message}");
        }
    }
}
