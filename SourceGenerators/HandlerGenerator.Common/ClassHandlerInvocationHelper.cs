using System.CodeDom.Compiler;

namespace HandlerGenerator.Common;

public static class ClassHandlerInvocationHelper
{
    public static void WriteClassHandlerInvocationDelegate(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.WriteLine("if (handlerMetadata is NonSingletonClassHandlerMetadata { Flags: var flags } nonSingletonHandlerMetadata)");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsAsyncDisposable))");
        writer.Indent++;

        WriteAsyncDisposableHandler(writer, handlerTypeName, handlerParameterNames);
        writer.Indent--;

        writer.WriteLine("else if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsNotConcrete))");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsDisposable))");
        writer.Indent++;

        WriteDisposableNotConcreteHandler(writer, handlerTypeName, handlerParameterNames);
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        WriteNotConcreteHandler(writer, handlerTypeName, handlerParameterNames);
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("else");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("if (flags.HasFlag(global::NetCord.Hosting.HandlerFlags.IsDisposable))");
        writer.Indent++;

        WriteDisposableConcreteHandler(writer, handlerTypeName, handlerParameterNames);
        writer.Indent--;

        writer.WriteLine("else");
        writer.Indent++;

        WriteConcreteHandler(writer, handlerTypeName, handlerParameterNames);
        writer.Indent--;

        writer.Indent--;
        writer.WriteLine("}");
        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("else");

        writer.Indent++;
        writer.Write("handler = ((");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(services)).HandleAsync;");

        writer.Indent--;
    }

    private static void WriteAsyncDisposableHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = (");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(scope.ServiceProvider);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("await ((IAsyncDisposable)instance).DisposeAsync().ConfigureAwait(false);");

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

    private static void WriteDisposableConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = (");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(scope.ServiceProvider);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(").ConfigureAwait(false);");

        writer.Indent--;
        writer.WriteLine("}");

        writer.WriteLine("finally");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("((IDisposable)instance).Dispose();");

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

    private static void WriteConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = (");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(scope.ServiceProvider);");

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameterNames(writer, handlerParameterNames);
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

    private static void WriteDisposableNotConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = (");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(scope.ServiceProvider);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameterNames(writer, handlerParameterNames);
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

        writer.WriteLine("((IDisposable)instance).Dispose();");
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

    private static void WriteNotConcreteHandler(IndentedTextWriter writer, string handlerTypeName, IReadOnlyList<string> handlerParameterNames)
    {
        writer.Write("handler = async (");
        Helper.WriteParameterNames(writer, handlerParameterNames);
        writer.WriteLine(") =>");
        writer.WriteLine("{");
        writer.Indent++;

        writer.WriteLine("var scope = global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateAsyncScope(services);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("var instance = (");
        writer.Write(handlerTypeName);
        writer.WriteLine(")instanceFactory(scope.ServiceProvider);");

        writer.WriteLine("try");
        writer.WriteLine("{");
        writer.Indent++;

        writer.Write("await instance.HandleAsync(");
        Helper.WriteParameterNames(writer, handlerParameterNames);
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
