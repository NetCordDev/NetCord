using System.Xml;

using Microsoft.CodeAnalysis;

namespace Shared;

public static class SymbolExtensions
{
    public static IReadOnlyList<string> GetXmlCommentLines(this ISymbol symbol, Func<XmlNode, bool> predicate)
    {
        var comment = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(comment))
            return [];

        XmlDocument xmlDocument = new();
        xmlDocument.LoadXml(comment);

        List<string> lines = [];

        foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
        {
            if (!predicate(xmlNode))
                continue;

            using StringReader stringReader = new(xmlNode.OuterXml);

            while (stringReader.ReadLine() is { } line)
                lines.Add(line);
        }

        return lines;
    }
}

