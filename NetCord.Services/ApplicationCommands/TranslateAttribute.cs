namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Field)]
public class TranslateAttribute : Attribute
{
    public Type TranslateProviderType { get; }

    public TranslateAttribute(Type translateProviderType)
    {
        if (!translateProviderType.IsAssignableTo(typeof(ITranslateProvider)))
            throw new ArgumentException($"Parameter must inherit from {nameof(ITranslateProvider)}", nameof(translateProviderType));

        TranslateProviderType = translateProviderType;
    }
}