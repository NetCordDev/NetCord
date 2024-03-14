using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using NetCord.Rest;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class EvalCommand : CommandModule<CommandContext>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(Discord.SerializerOptions) { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    [Command("eval", Priority = 0)]
    public async Task Eval([CommandParameter(Remainder = true)] string code)
    {
        if (Context.User.Id != 484036895391875093)
        {
            await ReplyAsync("You cannot use it!");
            return;
        }

        object? value;
        try
        {
            value = await CSharpScript.EvaluateAsync(code, ScriptOptions.Default.AddReferences(Assembly.GetEntryAssembly()).WithLanguageVersion(LanguageVersion.Preview).WithAllowUnsafe(true).AddImports(
                "NetCord",
                "NetCord.Rest",
                "NetCord.Gateway",
                "System",
                "System.Linq",
                "System.Threading",
                "System.Threading.Tasks"
            ), this, typeof(CommandModule<CommandContext>));
        }
        catch (RestException ex)
        {
            var error = ex.Error;
            throw new($"{ex.Message}\n{(error is null ? "No error returned." : new CodeBlock(JsonSerializer.Serialize(error, _jsonSerializerOptions), "json"))}");
        }
        if (value is not null)
        {
            List<EmbedFieldProperties> fields = [];
            foreach (var property in value.GetType().GetProperties().Take(24))
            {
                string description;
                try
                {
                    var v = property.GetValue(value);
                    description = v is not null ? (v.ToString() ?? "null") : "null";
                }
                catch (Exception ex)
                {
                    description = $"Exception was thrown: {ex}";
                }
                fields.Add(new() { Name = GetMaxLength(property.Name, 256), Value = GetMaxLength(description, 1024), Inline = true });
            }
            EmbedProperties embed = new()
            {
                Title = GetMaxLength($"Result: {value}", 256),
                Fields = fields,
            };

            MessageProperties message = new()
            {
                Embeds =
                [
                    embed,
                ],
            };
            await SendAsync(message);

            static string GetMaxLength(string text, int max)
                => text.Length > max ? text[..max] : text;
        }
    }

    [Command("eval", Priority = 1)]
    public Task Eval([CommandParameter(Remainder = true)] CodeBlock codeBlock)
        => Eval(codeBlock.Code);
}
