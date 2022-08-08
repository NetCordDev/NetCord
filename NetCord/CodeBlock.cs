using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public class CodeBlock
{
    public string Code { get; }
    public string? Formatter { get; }

    public CodeBlock(string code, string? formatter = null)
    {
        Code = code;
        Formatter = formatter;
    }

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out CodeBlock? codeBlock)
    {
        if (text.StartsWith("```") && text.EndsWith("```"))
        {
            text = text[3..^3];
            int i = text.IndexOf('\n');
            if (i != -1)
            {
                ReadOnlySpan<char> formatter = text[..i];
                foreach (var c in formatter)
                {
                    if (c is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '+' or '-'))
                    {
                        codeBlock = new(text.ToString());
                        return true;
                    }
                }
                codeBlock = new(text[(formatter.Length + 1)..].ToString(), formatter.ToString());
                return true;
            }
            else
            {
                codeBlock = new(text.ToString());
                return true;
            }
        }
        codeBlock = null;
        return false;
    }

    public static CodeBlock Parse(ReadOnlySpan<char> text)
    {
        if (TryParse(text, out var codeBlock))
            return codeBlock;
        else
            throw new FormatException($"Cannot parse '{nameof(CodeBlock)}'.");
    }

    public override string ToString() => $"```{Formatter}\n{Code}```";
}