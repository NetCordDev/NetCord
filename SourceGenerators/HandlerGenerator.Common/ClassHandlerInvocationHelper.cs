using System.CodeDom.Compiler;

namespace HandlerGenerator.Common;

public static class ClassHandlerInvocationHelper
{
    public static void WriteClassHandlerInvocationDelegate(IndentedTextWriter writer, string handlerTypeName, string handlerBaseTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        Helper.WriteHandlerDefinition(writer, handlerParameters.Select(p => p.Type));

        writer.Write("if (handlerMetadata is NonSingletonClassHandlerMetadata<");
        writer.Write(handlerBaseTypeName);
        writer.WriteLine("> { Flags: var flags } nonSingletonHandlerMetadata)");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsAsyncDisposable))");
        writer.Indent++;

        WriteAsyncDisposableHandler(writer, handlerTypeName, handlerParameters);
        writer.Indent--;

        writer.WriteLine("else if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsNotConcrete))");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsDisposable))");
        writer.Indent++;

        WriteDisposableNotConcreteHandler(writer, handlerTypeName, handlerParameters);
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        WriteNotConcreteHandler(writer, handlerTypeName, handlerParameters);
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("else");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsDisposable))");
        writer.Indent++;

        WriteDisposableConcreteHandler(writer, handlerTypeName, handlerParameters);
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        WriteConcreteHandler(writer, handlerTypeName, handlerParameters);
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");
        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("else");

        writer.Indent++;
        writer.Write("handler = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(services)");
        writer.WriteLine(".HandleAsync;");

        writer.Indent--;
    }

    private static void WriteAsyncDisposableHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(scope.ServiceProvider)");
        writer.WriteLine(";");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await ");
        Helper.WriteUnsafeAs(writer, "global::System.IAsyncDisposable", "instance");
        writer.WriteLine(".DisposeAsync().ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

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

    private static void WriteDisposableConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(scope.ServiceProvider)");
        writer.WriteLine(";");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        Helper.WriteUnsafeAs(writer, "global::System.IDisposable", "instance");
        writer.WriteLine(".Dispose();");

        writer.Indent--;
        writer.WriteLine("}");

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

    private static void WriteConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(scope.ServiceProvider)");
        writer.WriteLine(";");

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
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

    private static void WriteDisposableNotConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(scope.ServiceProvider)");
        writer.WriteLine(";");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (instance is IAsyncDisposable asyncDisposable)");
        writer.Indent++;

        writer.WriteLine("await asyncDisposable.DisposeAsync().ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("else");
        writer.Indent++;

        Helper.WriteUnsafeAs(writer, "global::System.IDisposable", "instance");
        writer.WriteLine(".Dispose();");
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");

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

    private static void WriteNotConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<HandlerParameter> handlerParameters)
    {
        writer.Write("handler = async (");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = ");
        Helper.WriteUnsafeAs(writer, handlerTypeName, "instanceFactory(scope.ServiceProvider)");
        writer.WriteLine(";");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameters(writer, handlerParameters.Select(p => p.Name));
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (instance is IAsyncDisposable asyncDisposable)");
        writer.Indent++;

        writer.WriteLine("await asyncDisposable.DisposeAsync().ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("else if (instance is IDisposable disposable)");
        writer.Indent++;

        writer.WriteLine("disposable.Dispose();");
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");

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
