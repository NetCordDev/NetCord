using System.CodeDom.Compiler;

namespace HandlerGenerator.Common;

public static class DelegateHandlerInvocationHelper
{
    public static void WriteDelegateHandlerInvocationDelegate(IndentedTextWriter writer, IReadOnlyList<string> handlerParameterNames)
    {
        writer.WriteLine("if (handlerMetadata.IsSingleton)");
        writer.Indent++;

        writer.Write("handler = (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.Write(") => rawHandler(");
        Helper.WriteParameterNames(writer, [.. handlerParameterNames, "services"]);
        writer.WriteLine(");");
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.Write(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");

        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await rawHandler(");
        Helper.WriteParameterNames(writer, [.. handlerParameterNames, "scope.ServiceProvider"]);
        writer.WriteLine(").ConfigureAwait(false);");
        writer.Indent--;

        writer.WriteLine("}");

        writer.WriteLine("finally");

        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("await scope.DisposeAsync().ConfigureAwait(false);");
        writer.Indent--;

        writer.WriteLine("}");
        writer.Indent--;

        writer.WriteLine("};");
    }
}

