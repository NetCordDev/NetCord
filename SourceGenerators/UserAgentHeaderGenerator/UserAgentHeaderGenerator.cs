using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using Shared;

namespace UserAgentHeaderGenerator;

[Generator(LanguageNames.CSharp)]
public class UserAgentHeaderGenerator : IIncrementalGenerator
{
    private const string AssemblyInformationalVersionAttributeQualifiedName = "System.Reflection.AssemblyInformationalVersionAttribute";

    private const string ProjectUrl = "https://netcord.dev";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var version = context.CompilationProvider.Select((compilation, cancellationToken) =>
        {
            var fullVersion = (string)compilation.Assembly.GetAttributes().First(data => data.AttributeClass!.ToQualifiedName() == AssemblyInformationalVersionAttributeQualifiedName).ConstructorArguments[0].Value!;
            return fullVersion[..fullVersion.LastIndexOf('+')];
        });

        context.RegisterSourceOutput(version, (context, version) =>
        {
            context.AddSource("RestClient.g.cs", SourceText.From(
                $$"""
                namespace NetCord.Rest;

                public partial class RestClient
                {
                    private const string UserAgentHeader = "DiscordBot ({{version}}, {{ProjectUrl}})";
                }
                """, Encoding.UTF8));
        });
    }
}
