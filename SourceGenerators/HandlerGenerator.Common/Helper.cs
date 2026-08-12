using System.CodeDom.Compiler;

namespace HandlerGenerator.Common;

internal static class Helper
{
    public static void WriteParameterNames(IndentedTextWriter writer, IReadOnlyList<string> parameterNames)
    {
        var count = parameterNames.Count;

        for (int i = 0; i < count; i++)
        {
            writer.Write(parameterNames[i]);

            if (i < count - 1)
                writer.Write(", ");
        }
    }
}

