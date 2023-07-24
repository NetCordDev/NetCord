namespace MethodsForPropertiesGenerator;

internal static class StringWriterExtensions
{
    public static unsafe void Write(this StringWriter stringWriter, ReadOnlySpan<char> value)
    {
        fixed (char* ptr = value)
            stringWriter.GetStringBuilder().Append(ptr, value.Length);
    }
}
