using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // Inline so that strictMode branches can be eliminated if it is a constant
    public static bool TryParse(ReadOnlySpan<char> text, [NotNullWhen(true)] out CodeBlock? codeBlock, bool strictMode = true)
    {
        const string prefixSuffix = "```";
        
        codeBlock = null;
        
        var isCodeBlock = text.Length >= $"{prefixSuffix}\n{prefixSuffix}".Length && text.StartsWith(prefixSuffix) && text.EndsWith(prefixSuffix);
        
        if (!isCodeBlock)
        {
            goto Ret;
        }
        
        string? formatter = null;
        text = text[prefixSuffix.Length..^prefixSuffix.Length];
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
