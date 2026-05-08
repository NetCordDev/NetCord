using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace ServicesTest;

[TestClass]
public class SlashCommandGroupTests
{
    private static ApplicationCommandService<ApplicationCommandContext> GetService()
    {
        return new();
    }

    [SlashCommand("not-empty", "not-empty")]
    private class NotEmptyModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("test", "test")]
        public static void TestCommand()
        {
        }
    }

    [SlashCommand("not-empty2", "not-empty2")]
    private class NotEmptyModule2 : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("sub", "sub")]
        public class NotEmptySubModule : ApplicationCommandModule<ApplicationCommandContext>
        {
            [SubSlashCommand("test", "test")]
            public static void TestCommand()
            {
            }
        }
    }

    [SlashCommand("empty", "empty")]
    private class EmptyModule : ApplicationCommandModule<ApplicationCommandContext>
    {
    }

    [SlashCommand("empty2", "empty2")]
    private class EmptyModule2 : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("sub", "sub")]
        public class EmptySubModule : ApplicationCommandModule<ApplicationCommandContext>
        {
        }
    }

    [TestMethod]
    public void NotEmptySlashCommandGroupMinimal()
    {
        var service = GetService();

        SlashCommandGroupBuilder groupBuilder = new SlashCommandGroupBuilder("test", "Test");

        groupBuilder.AddSubCommand("test", "Test", () => { });

        service.AddSlashCommandGroup(groupBuilder);
    }

    [TestMethod]
    public void NotEmptySlashCommandGroup()
    {
        var service = GetService();

        service.AddModule<NotEmptyModule>();
    }

    [TestMethod]
    public void NotEmptySubSlashCommandGroupMinimal()
    {
        var service = GetService();

        SlashCommandGroupBuilder groupBuilder = new SlashCommandGroupBuilder("test", "Test");

        var subGroupBuilder = groupBuilder.AddSubCommandGroup("sub", "Sub");

        subGroupBuilder.AddSubCommand("test", "Test", () => { });

        service.AddSlashCommandGroup(groupBuilder);
    }

    [TestMethod]
    public void NotEmptySubSlashCommandGroup()
    {
        var service = GetService();

        service.AddModule<NotEmptyModule2>();
    }

    [TestMethod]
    public void EmptySlashCommandGroupMinimal()
    {
        var service = GetService();

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommandGroup(new SlashCommandGroupBuilder("test", "Test"));
        });
    }

    [TestMethod]
    public void EmptySlashCommandGroup()
    {
        var service = GetService();

        Assert.ThrowsExactly<InvalidDefinitionException>(service.AddModule<EmptyModule>);
    }

    [TestMethod]
    public void EmptySubSlashCommandGroupMinimal()
    {
        var service = GetService();

        SlashCommandGroupBuilder groupBuilder = new SlashCommandGroupBuilder("test", "Test");

        groupBuilder.AddSubCommandGroup("sub", "Sub");

        Assert.ThrowsExactly<InvalidDefinitionException>(() =>
        {
            service.AddSlashCommandGroup(groupBuilder);
        });
    }

    [TestMethod]
    public void EmptySubSlashCommandGroup()
    {
        var service = GetService();

        Assert.ThrowsExactly<InvalidDefinitionException>(service.AddModule<EmptyModule2>);
    }
}
