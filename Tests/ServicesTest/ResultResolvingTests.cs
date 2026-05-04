using System.Diagnostics.CodeAnalysis;

using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

namespace ServicesTest;

[TestClass]
public class ResultResolvingTests
{
    private class DummyResult;

    private class TestResultResolverProvider<TContext>(HashSet<Type> resolvableTypes) : IResultResolverProvider<TContext>
    {
        public List<Type> ReceivedTypes { get; } = [];

        public bool TryGetResolver(Type type, [MaybeNullWhen(false)] out Func<object?, TContext, ValueTask> resolver)
        {
            ReceivedTypes.Add(type);

            if (resolvableTypes.Contains(type))
            {
                resolver = (_, _) => default;
                return true;
            }

            resolver = null;
            return false;
        }
    }

    [TestMethod]
    public void ApplicationCommandServiceValueTaskNoFallback()
    {
        var (service, resolverProvider) = CreateApplicationCommandService(typeof(ValueTask));

        service.AddSlashCommand(new SlashCommandBuilder("test", "test", () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
    }

    [TestMethod]
    public void ComponentInteractionServiceValueTaskNoFallback()
    {
        var (service, resolverProvider) = CreateComponentInteractionService(typeof(ValueTask));

        service.AddComponentInteraction(new ComponentInteractionBuilder("test", () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
    }

    [TestMethod]
    public void CommandServiceValueTaskNoFallback()
    {
        var (service, resolverProvider) = CreateCommandService(typeof(ValueTask));

        service.AddCommand(new CommandBuilder(["test"], () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
    }

    [TestMethod]
    public void ApplicationCommandServiceValueTaskTNoFallback()
    {
        var (service, resolverProvider) = CreateApplicationCommandService(typeof(ValueTask<DummyResult>));

        service.AddSlashCommand(new SlashCommandBuilder("test", "test", () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
    }

    [TestMethod]
    public void ComponentInteractionServiceValueTaskTNoFallback()
    {
        var (service, resolverProvider) = CreateComponentInteractionService(typeof(ValueTask<DummyResult>));

        service.AddComponentInteraction(new ComponentInteractionBuilder("test", () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
    }

    [TestMethod]
    public void CommandServiceValueTaskTNoFallback()
    {
        var (service, resolverProvider) = CreateCommandService(typeof(ValueTask<DummyResult>));

        service.AddCommand(new CommandBuilder(["test"], () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(1, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
    }

    [TestMethod]
    public void ApplicationCommandServiceTaskFallback()
    {
        var (service, resolverProvider) = CreateApplicationCommandService(typeof(Task));

        service.AddSlashCommand(new SlashCommandBuilder("test", "test", () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
        Assert.AreEqual(typeof(Task), receivedTypes[1]);
    }

    [TestMethod]
    public void ComponentInteractionServiceTaskFallback()
    {
        var (service, resolverProvider) = CreateComponentInteractionService(typeof(Task));

        service.AddComponentInteraction(new ComponentInteractionBuilder("test", () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
        Assert.AreEqual(typeof(Task), receivedTypes[1]);
    }

    [TestMethod]
    public void CommandServiceTaskFallback()
    {
        var (service, resolverProvider) = CreateCommandService(typeof(Task));

        service.AddCommand(new CommandBuilder(["test"], () => default(ValueTask)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask), receivedTypes[0]);
        Assert.AreEqual(typeof(Task), receivedTypes[1]);
    }

    [TestMethod]
    public void ApplicationCommandServiceTaskTFallback()
    {
        var (service, resolverProvider) = CreateApplicationCommandService(typeof(Task<DummyResult>));

        service.AddSlashCommand(new SlashCommandBuilder("test", "test", () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
        Assert.AreEqual(typeof(Task<DummyResult>), receivedTypes[1]);
    }

    [TestMethod]
    public void ComponentInteractionServiceTaskTFallback()
    {
        var (service, resolverProvider) = CreateComponentInteractionService(typeof(Task<DummyResult>));

        service.AddComponentInteraction(new ComponentInteractionBuilder("test", () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
        Assert.AreEqual(typeof(Task<DummyResult>), receivedTypes[1]);
    }

    [TestMethod]
    public void CommandServiceTaskTFallback()
    {
        var (service, resolverProvider) = CreateCommandService(typeof(Task<DummyResult>));

        service.AddCommand(new CommandBuilder(["test"], () => default(ValueTask<DummyResult>)));

        var receivedTypes = resolverProvider.ReceivedTypes;

        Assert.HasCount(2, receivedTypes);
        Assert.AreEqual(typeof(ValueTask<DummyResult>), receivedTypes[0]);
        Assert.AreEqual(typeof(Task<DummyResult>), receivedTypes[1]);
    }

    private static (ApplicationCommandService<ApplicationCommandContext> Service, TestResultResolverProvider<ApplicationCommandContext> ResolverProvider) CreateApplicationCommandService(params HashSet<Type> resolvableTypes)
    {
        TestResultResolverProvider<ApplicationCommandContext> provider = new(resolvableTypes);
        return (new ApplicationCommandService<ApplicationCommandContext>(ApplicationCommandServiceConfiguration<ApplicationCommandContext>.Default with
        {
            ResultResolverProvider = provider,
        }), provider);
    }

    private static (ComponentInteractionService<ComponentInteractionContext> Service, TestResultResolverProvider<ComponentInteractionContext> ResolverProvider) CreateComponentInteractionService(params HashSet<Type> resolvableTypes)
    {
        TestResultResolverProvider<ComponentInteractionContext> provider = new(resolvableTypes);
        return (new ComponentInteractionService<ComponentInteractionContext>(ComponentInteractionServiceConfiguration<ComponentInteractionContext>.Default with
        {
            ResultResolverProvider = provider,
        }), provider);
    }

    private static (CommandService<CommandContext> Service, TestResultResolverProvider<CommandContext> ResolverProvider) CreateCommandService(params HashSet<Type> resolvableTypes)
    {
        TestResultResolverProvider<CommandContext> provider = new(resolvableTypes);
        return (new CommandService<CommandContext>(CommandServiceConfiguration<CommandContext>.Default with
        {
            ResultResolverProvider = provider,
        }), provider);
    }
}
