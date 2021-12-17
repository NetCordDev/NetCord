using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public class CodeBlock
{
    public string Code { get; }
    public string Formatter { get; }

    public CodeBlock(ReadOnlySpan<char> code, ReadOnlySpan<char> formatter = default)
    {
        Code = code.ToString();
        Formatter = formatter.ToString();
    }

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out CodeBlock? codeBlock)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

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
                        codeBlock = new(text);
                        return true;
                    }
                }
                codeBlock = new(text[(formatter.Length + 1)..], formatter);
                return true;
            }
            else
            {
                codeBlock = new(text);
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
            throw new FormatException($"Cannot parse {nameof(CodeBlock)}");
    }

    public override string ToString() => $"```{Formatter}\n{Code}```";
}