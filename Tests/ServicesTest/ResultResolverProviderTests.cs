using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
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
        {
            Assert.IsTrue(provider.TryGetResolver(type, out var resolver), $"Failed to get resolver for {type}");
            Assert.IsNotNull(resolver, $"Resolver for {type} is null");
        }
    }
}
