﻿using System.Xml;

using Microsoft.CodeAnalysis;

namespace Shared;

public static class StringWriterExtensions
{
    private const string Indentation = "    ";

    private const int IndentationLength = 4;

    public static unsafe void Write(this StringWriter stringWriter, ReadOnlySpan<char> value)
    {
        fixed (char* ptr = value)
            stringWriter.GetStringBuilder().Append(ptr, value.Length);
    }

    public static void WriteNamespace(this StringWriter stringWriter, ITypeSymbol typeSymbol)
    {
        var containingNamespace = typeSymbol.ContainingNamespace;
        if (containingNamespace.IsGlobalNamespace)
            return;

        stringWriter.Write("namespace ");
        stringWriter.Write(containingNamespace.ToDisplayString());
        stringWriter.WriteLine(";");
    }

    public static void WriteTypeDeclaration(this StringWriter stringWriter, ITypeSymbol typeSymbol)
    {
        stringWriter.Write("partial ");
        stringWriter.Write(typeSymbol switch
        {
            { TypeKind: TypeKind.Interface } => "interface ",
            { IsReferenceType: true, IsRecord: false } => "class ",
            { IsReferenceType: true, IsRecord: true } => "record ",
            { IsValueType: true, IsRecord: false } => "struct ",
            { IsValueType: true, IsRecord: true } => "record struct ",
            _ => throw new ArgumentException($"Unsupported type kind: {typeSymbol.TypeKind}", nameof(typeSymbol)),
        });
        stringWriter.WriteLine(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
    }

    public static unsafe void WriteXmlComment(this StringWriter stringWriter, ISymbol symbol)
    {
        stringWriter.CopyXmlComment(symbol, _ => true);
    }

    public static unsafe void CopyXmlComment(this StringWriter stringWriter, ISymbol symbol, Func<XmlNode, bool> predicate)
    {
        var comment = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(comment))
            return;

        XmlDocument xmlDocument = new();
        xmlDocument.LoadXml(comment);

        foreach (XmlNode xmlNode in xmlDocument.DocumentElement!.ChildNodes)
        {
            if (!predicate(xmlNode))
                continue;

            using StringReader stringReader = new(xmlNode.OuterXml);
            string? line;
            while ((line = stringReader.ReadLine()) is not null)
            {
                stringWriter.WriteIndentation(1);
                stringWriter.Write("/// ");
                if (line.StartsWith(Indentation))
                    stringWriter.Write(line.AsSpan(IndentationLength));
                else
                    stringWriter.Write(line);

                stringWriter.WriteLine();
            }
        }
    }

    public static void WriteAutoGeneratedComment(this StringWriter stringWriter)
    {
        stringWriter.WriteLine("// <auto-generated/>");
    }

    public static void WriteNullableDirective(this StringWriter stringWriter)
    {
        stringWriter.WriteLine("#nullable enable");
    }

    public static void WriteIndentation(this StringWriter stringWriter, int indentation)
    {
        for (int i = 0; i < indentation; i++)
            stringWriter.Write(Indentation);
    }
}
