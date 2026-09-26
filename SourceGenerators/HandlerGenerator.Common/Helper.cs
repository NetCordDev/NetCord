namespace HandlerGenerator.Common;

internal static class Helper
{
    public static void WriteHandlerDefinition(TextWriter writer, IEnumerable<string> parameters)
    {
        writer.Write("global::System.Func<");
        WriteParametersWithCommaAtEnd(writer, parameters);
        writer.WriteLine("global::System.Threading.Tasks.ValueTask> handler;");
    }

    public static void WriteParameters(TextWriter writer, IEnumerable<string> parameters)
    {
        using var enumerator = parameters.GetEnumerator();

        if (!enumerator.MoveNext())
            return;

        writer.Write(enumerator.Current);

        while (enumerator.MoveNext())
        {
            writer.Write(", ");
            writer.Write(enumerator.Current);
        }
    }

    public static void WriteParametersWithCommaAtEnd(TextWriter writer, IEnumerable<string> parameters)
    {
        foreach (var parameter in parameters)
        {
            writer.Write(parameter);
            writer.Write(", ");
        }
    }

    public static void WriteUnsafeAs(TextWriter writer, string typeName, string obj)
    {
        writer.Write("global::System.Runtime.CompilerServices.Unsafe.As<");
        writer.Write(typeName);
        writer.Write(">(");
        writer.Write(obj);
        writer.Write(")");
    }

    public static void WriteUnsafeAs(TextWriter writer, Action<TextWriter> writeTypeName, string obj)
    {
        writer.Write("global::System.Runtime.CompilerServices.Unsafe.As<");
        writeTypeName(writer);
        writer.Write(">(");
        writer.Write(obj);
        writer.Write(")");
    }
}

