using System.CodeDom.Compiler;

using Microsoft.CodeAnalysis;

namespace HandlerGenerator.Common;

public static class DelegateHandlerInvocationHelper
{
    public static void WriteDelegateHandlerInvocationDelegate(IndentedTextWriter writer, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("var rawHandler = ");
        Helper.WriteUnsafeAs(writer, w =>
        {
            w.Write("global::System.Func<");
            Helper.WriteParametersWithCommaAtEnd(w, handlerParameters.Select(p => p.Type));
            w.Write("global::System.IServiceProvider, global::System.Threading.Tasks.ValueTask>");
        }, "handlerMetadata.Handler");
        writer.WriteLine(";");

        Helper.WriteHandlerDefinition(writer, handlerParameters.Select(p => p.Type));

        writer.WriteLine("if (handlerMetadata.IsSingleton)");
        writer.Indent++;

        writer.Write("handler = (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.Write(") => rawHandler(");
        Helper.WriteParameters(writer, [.. handlerParameters.Select(p => p.Name), "services"]);
        writer.WriteLine(");");
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");

        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");

        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await rawHandler(");
        Helper.WriteParameters(writer, [.. handlerParameters.Select(p => p.Name), "scope.ServiceProvider"]);
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
        writer.Indent--;
    }
}
