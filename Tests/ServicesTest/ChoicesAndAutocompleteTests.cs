using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace ServicesTest;

[TestClass]
public class ChoicesAndAutocompleteTests
{
    private static ApplicationCommandService<ApplicationCommandContext, AutocompleteInteractionContext> CreateService()
    {
        return new ApplicationCommandService<ApplicationCommandContext, AutocompleteInteractionContext>();
    }

    private enum TestEnum
    {
        Value1,
        Value2,
        Value3,
    }

    private class TestChoicesProvider : IChoicesProvider<ApplicationCommandContext>
    {
        public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(SlashCommandParameter<ApplicationCommandContext> parameter)
        {
            return new([]);
        }
    }

    private class TestAutocompleteProvider : IAutocompleteProvider<AutocompleteInteractionContext>
    {
        public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
        {
            return new([]);
        }
    }

    private class ChoicesProviderTypeReader : SlashCommandTypeReader<ApplicationCommandContext>
    {
        public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.String;

        public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, ApplicationCommandContext context, SlashCommandParameter<ApplicationCommandContext> parameter, ApplicationCommandServiceConfiguration<ApplicationCommandContext> configuration, IServiceProvider? serviceProvider)
        {
            return new(SlashCommandTypeReaderResult.Success(value));
        }

        public override IChoicesProvider<ApplicationCommandContext>? ChoicesProvider => new TestChoicesProvider();
    }

    private class AutocompleteProviderTypeReader : SlashCommandTypeReader<ApplicationCommandContext>
    {
        public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.String;

        public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, ApplicationCommandContext context, SlashCommandParameter<ApplicationCommandContext> parameter, ApplicationCommandServiceConfiguration<ApplicationCommandContext> configuration, IServiceProvider? serviceProvider)
        {
            return new(SlashCommandTypeReaderResult.Success(value));
        }

        public override Type? AutocompleteProviderType => typeof(TestAutocompleteProvider);
    }

    private class InvalidTypeReader : SlashCommandTypeReader<ApplicationCommandContext>
    {
        public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.String;

        public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, ApplicationCommandContext context, SlashCommandParameter<ApplicationCommandContext> parameter, ApplicationCommandServiceConfiguration<ApplicationCommandContext> configuration, IServiceProvider? serviceProvider)
        {
            return new(SlashCommandTypeReaderResult.Success(value));
        }

        public override IChoicesProvider<ApplicationCommandContext>? ChoicesProvider => new TestChoicesProvider();

        public override Type? AutocompleteProviderType => typeof(TestAutocompleteProvider);
    }

    [TestMethod]
    public void TestChoices()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(ChoicesProviderType = typeof(TestChoicesProvider))] string s) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.IsExactInstanceOfType<TestChoicesProvider>(parameter.ChoicesProvider);
    }

    [TestMethod]
    public void TestDefaultEnumChoices()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            (TestEnum e) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.IsNotNull(parameter.ChoicesProvider);
    }

    [TestMethod]
    public void TestCustomEnumChoices()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(ChoicesProviderType = typeof(TestChoicesProvider))] TestEnum e) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.IsExactInstanceOfType<TestChoicesProvider>(parameter.ChoicesProvider);
    }

    [TestMethod]
    public void TestEnumAutocomplete()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(AutocompleteProviderType = typeof(TestAutocompleteProvider))] TestEnum e) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.AreEqual(typeof(TestAutocompleteProvider), parameter.AutocompleteProviderType);

        Assert.IsNull(parameter.ChoicesProvider);
    }

    [TestMethod]
    public void TestAutocomplete()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(AutocompleteProviderType = typeof(TestAutocompleteProvider))] string s) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.AreEqual(typeof(TestAutocompleteProvider), parameter.AutocompleteProviderType);
    }

    [TestMethod]
    public void TestChoicesOverride()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(TypeReaderType = typeof(AutocompleteProviderTypeReader),
                                    ChoicesProviderType = typeof(TestChoicesProvider))] string s) =>
            { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.IsExactInstanceOfType<TestChoicesProvider>(parameter.ChoicesProvider);

        Assert.IsNull(parameter.AutocompleteProviderType);
    }

    [TestMethod]
    public void TestAutocompleteOverride()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            ([SlashCommandParameter(TypeReaderType = typeof(ChoicesProviderTypeReader),
                                    AutocompleteProviderType = typeof(TestAutocompleteProvider))] string s) =>
            { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.AreEqual(typeof(TestAutocompleteProvider), parameter.AutocompleteProviderType);

        Assert.IsNull(parameter.ChoicesProvider);
    }

    [TestMethod]
    public void TestChoicesAndAutocomplete()
    {
        var service = CreateService();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommand(new SlashCommandBuilder(
                "test",
                "Test",
                ([SlashCommandParameter(ChoicesProviderType = typeof(TestChoicesProvider),
                                        AutocompleteProviderType = typeof(TestAutocompleteProvider))] string s) =>
                { }));
        });
    }

    [TestMethod]
    public void TestInvalidTypeReader()
    {
        var service = CreateService();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommand(new SlashCommandBuilder(
                "test",
                "Test",
                ([SlashCommandParameter(TypeReaderType = typeof(InvalidTypeReader))] string s) => { }));
        });
    }

    [TestMethod]
    public void TestInvalidChoicesProvider()
    {
        var service = CreateService();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommand(new SlashCommandBuilder(
                "test",
                "Test",
                ([SlashCommandParameter(ChoicesProviderType = typeof(string))] string s) => { }));
        });
    }

    [TestMethod]
    public void TestInvalidAutocompleteProvider()
    {
        var service = CreateService();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommand(new SlashCommandBuilder(
                "test",
                "Test",
                ([SlashCommandParameter(AutocompleteProviderType = typeof(string))] string s) => { }));
        });
    }

    [TestMethod]
    public void TestAutocompleteNotSupported()
    {
        ApplicationCommandService<ApplicationCommandContext> service = new();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommand(new SlashCommandBuilder(
                "test",
                "Test",
                ([SlashCommandParameter(AutocompleteProviderType = typeof(TestAutocompleteProvider))] int i) => { }));
        });
    }

    private enum TestEnumWithIgnoredValues
    {
        Value1,
        [SlashCommandIgnore]
        Value2,
        Value3,
    }

    [TestMethod]
    public async Task TestEnumIgnoreAttribute()
    {
        var service = CreateService();

        service.AddSlashCommand(new SlashCommandBuilder(
            "test",
            "Test",
            (TestEnumWithIgnoredValues e) => { }));

        var command = (SlashCommandInfo<ApplicationCommandContext>)service.GetCommands().Single();

        var parameter = command.Parameters.Single();

        Assert.IsNotNull(parameter.ChoicesProvider);

        var choices = await parameter.ChoicesProvider.GetChoicesAsync(parameter).ConfigureAwait(false);

        Assert.IsNotNull(choices);

        var choicesList = choices.ToList();

        Assert.AreEqual(2, choicesList.Count);
        Assert.AreEqual("Value1", choicesList[0].Name);
        Assert.AreEqual("Value3", choicesList[1].Name);
    }
}
