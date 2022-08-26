namespace NetCord.Services;

internal static class TypeReaderAttributeHelper
{
    public static TTypeReader GetTypeReader<TContext, TTypeReaderInterface, TTypeReader>(TypeReaderAttribute attribute) where TTypeReader : TTypeReaderInterface
    {
        var typeReaderType = attribute.TypeReaderType;
        var typeReader = Activator.CreateInstance(typeReaderType);

        if (typeReader is not TTypeReaderInterface iTypeReader)
            throw new InvalidOperationException($"'{typeReaderType.FullName}' must inherit from '{typeof(TTypeReader).FullName}'.");

        if (iTypeReader is TTypeReader castedTypeReader)
            return castedTypeReader;
        else
            throw new InvalidOperationException($"Context of '{typeReaderType.FullName}' is not convertible to '{typeof(TContext)}'.");
    }
}
