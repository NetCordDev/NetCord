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

    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out CodeBlock? codeBlock, bool strictMode = true)
    {
        codeBlock = null;

        var isCodeBlock = text.StartsWith("```") && text.EndsWith("```") && text.Length >= "```\n```".Length;
        
        if (!isCodeBlock)
        {
            goto Ret;
        }
        
        string? formatter = null;
        text = text[3..^3];
        var firstNewLine = text.IndexOf('\n');
            
        if (firstNewLine != -1)
        {                
            ReadOnlySpan<char> formatterSpan = text[..firstNewLine];
                
            foreach (var c in formatterSpan)
            {
                var isAsciiAlphaNumeric = char.IsAsciiLetterOrDigit(c);
                    
                if (isAsciiAlphaNumeric || c == '+' || c == '-')
                {
                    continue;
                }
                    
                goto Success;
            }
            
            text = text[(formatterSpan.Length + 1)..];
            
            if (!strictMode || !text.IsWhiteSpace())
            {
                formatter = formatterSpan.ToString();
            }
				
            else
            {
                text = formatterSpan;
            }
        }
        
        Success:
        codeBlock = new(text.ToString(), formatter);
        
        Ret:
        return isCodeBlock;
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
