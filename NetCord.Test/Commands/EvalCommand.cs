using System.Reflection;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using NetCord.Rest;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class EvalCommand : CommandModule
{
    [Command("eval", Priority = 0)]
    public async Task Eval([Remainder] string code)
    {
        if (Context.User.Id != 484036895391875093)
        {
            await ReplyAsync("You cannot use it!");
            return;
        }

        object? value;
        try
        {
            value = await CSharpScript.EvaluateAsync(code, ScriptOptions.Default.AddReferences(Assembly.GetEntryAssembly()), this, typeof(CommandModule));
        }
        catch (RestException ex)
        {
            throw new($"{ex.Message}\n{new CodeBlock(await ex.GetDiscordErrorMessageAsync(), "json")}");
        }
        if (value != null)
        {
            List<EmbedFieldProperties> fields = new();
            foreach (var property in value.GetType().GetProperties().Take(24))
            {
                string description;
                try
                {
                    var v = property.GetValue(value);
                    description = v != null ? v.ToString()! : "null";
                }
                catch (Exception ex)
                {
                    description = $"Exception was thrown: {ex}";
                }
                fields.Add(new() { Title = GetMaxLength(property.Name, 256), Description = GetMaxLength(description, 1024), Inline = true });
            }
            EmbedProperties embed = new()
            {
                Title = GetMaxLength($"Result: {value}", 256),
                Fields = fields,
            };

            MessageProperties message = new()
            {
                Embeds = new List<EmbedProperties>()
                {
                    embed
                }
            };
            await SendAsync(message);

            static string GetMaxLength(string text, int max)
                => text.Length > max ? text[..max] : text;
        }
    }

    [Command("eval", Priority = 1)]
    public Task Eval([Remainder] CodeBlock codeBlock)
        => Eval(codeBlock.Code);
}