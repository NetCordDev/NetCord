using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

namespace ServicesTest;

[TestClass]
public class ResultResolverProviderTests
{
    [TestMethod]
    public void TestApplicationCommandResultResolverProvider()
    {
        TestInteractionResultResolverProvider(ApplicationCommandResultResolverProvider<IApplicationCommandContext>.Instance);
    }

    [TestMethod]
    public void TestComponentInteractionResultResolverProvider()
    {
        TestInteractionResultResolverProvider(ComponentInteractionResultResolverProvider<IComponentInteractionContext>.Instance);
    }

    private static void TestInteractionResultResolverProvider<TContext>(IResultResolverProvider<TContext> provider)
    {
        IReadOnlyList<Type> types = [
            typeof(Task),
            typeof(Task<InteractionCallbackProperties<InteractionMessageProperties>>),
            typeof(Task<InteractionCallbackProperties<MessageOptions>>),
            typeof(Task<InteractionCallbackProperties<InteractionCallbackChoicesDataProperties>>),
            typeof(Task<InteractionCallbackProperties<ModalProperties>>),
            typeof(Task<InteractionCallbackProperties>),
            typeof(Task<InteractionMessageProperties>),
            typeof(Task<string>),
            typeof(Task<ModalProperties>),
            typeof(void),
            typeof(InteractionCallbackProperties<InteractionMessageProperties>),
            typeof(InteractionCallbackProperties<MessageOptions>),
            typeof(InteractionCallbackProperties<InteractionCallbackChoicesDataProperties>),
            typeof(InteractionCallbackProperties<ModalProperties>),
            typeof(InteractionCallbackProperties),
            typeof(InteractionMessageProperties),
            typeof(string),
            typeof(ModalProperties),
        ];

        foreach (var type in types)
            Validate(provider, type);
    }

    [TestMethod]
    public void TestCommandResultResolverProvider()
    {
        IReadOnlyList<Type> types = [
            typeof(Task),
            typeof(Task<ReplyMessageProperties>),
            typeof(Task<MessageProperties>),
            typeof(Task<string>),
            typeof(void),
            typeof(ReplyMessageProperties),
            typeof(MessageProperties),
            typeof(string),
        ];

        var provider = CommandResultResolverProvider<CommandContext>.Instance;

        foreach (var type in types)
            Validate(provider, type);
    }

    private static void Validate<TContext>(IResultResolverProvider<TContext> provider, Type type)
    {
        Assert.IsTrue(provider.TryGetResolver(type, out var resolver), $"Failed to get resolver for {type}");
        Assert.IsNotNull(resolver, $"Resolver for {type} is null");
    }
}
