using NetCord;
using NetCord.JsonModels;
using NetCord.Rest;

namespace RoleTest;

[TestClass]
public class GuildUserComparison
{
    private static RestGuild CreateGuild(ulong ownerId, params (ulong Id, int Position)[] roles)
    {
        var length = roles.Length;
        var jsonRoles = new JsonRole[length];

        for (int i = 0; i < length; i++)
        {
            var (id, position) = roles[i];
            jsonRoles[i] = new()
            {
                Id = id,
                Position = position,
            };
        }

        return new(new()
        {
            OwnerId = ownerId,
            Roles = jsonRoles,
        }, null!);
    }

    private static PartialGuildUser CreateUser(ulong id, params ulong[] roleIds)
    {
        return new(new()
        {
            User = new()
            {
                Id = id,
            },
            RoleIds = roleIds,
        }, null!);
    }

    private static void Compare(RestGuild guild, PartialGuildUser? user1, PartialGuildUser? user2, bool user1Greater)
    {
        if (user1Greater)
        {
            Assert.IsGreaterThan(0, guild.Compare(user1, user2));
            Assert.IsLessThan(0, guild.Compare(user2, user1));
        }
        else
        {
            Assert.IsLessThan(0, guild.Compare(user1, user2));
            Assert.IsGreaterThan(0, guild.Compare(user2, user1));
        }
    }

    private static void Equal(RestGuild guild, PartialGuildUser? user1, PartialGuildUser? user2)
    {
        Assert.AreEqual(0, guild.Compare(user1, user2));
        Assert.AreEqual(0, guild.Compare(user2, user1));
    }

    [TestMethod]
    public void ReferenceEqual()
    {
        var guild = CreateGuild(0);
        var user = CreateUser(1);

        Equal(guild, user, user);
    }

    [TestMethod]
    public void IdEqual()
    {
        var guild = CreateGuild(0);
        var user1 = CreateUser(1);
        var user2 = CreateUser(1);

        Equal(guild, user1, user2);
    }

    [TestMethod]
    public void NullCompare()
    {
        var guild = CreateGuild(0);
        var user = CreateUser(1);

        Compare(guild, user, null, true);
        Equal(guild, null, null);
    }

    [TestMethod]
    public void OwnerCompare()
    {
        var guild = CreateGuild(1);
        var owner = CreateUser(1);
        var user = CreateUser(2);

        Compare(guild, owner, user, true);
    }

    [TestMethod]
    public void NoRolesCompare()
    {
        var guild = CreateGuild(0, (100, 1));
        var userWithRole = CreateUser(1, 100);
        var userNoRole = CreateUser(2);

        Compare(guild, userWithRole, userNoRole, true);
    }

    [TestMethod]
    public void NoRolesMultipleCountCompare()
    {
        var guild = CreateGuild(0, (100, 1), (101, 2));
        var userWithRoles = CreateUser(1, 100, 101);
        var userNoRole = CreateUser(2);

        Compare(guild, userWithRoles, userNoRole, true);
    }

    [TestMethod]
    public void RolePositionCompare()
    {
        var guild = CreateGuild(0, (100, 2), (101, 1));
        var userHigh = CreateUser(1, 100);
        var userLow = CreateUser(2, 101);

        Compare(guild, userHigh, userLow, true);
    }

    [TestMethod]
    public void MultipleRolesCompare()
    {
        var guild = CreateGuild(0, (100, 2), (101, 1));
        var user1 = CreateUser(1, 100, 101);
        var user2 = CreateUser(2, 101);

        Compare(guild, user1, user2, true);
    }

    [TestMethod]
    public void SameHighestRoleCompare()
    {
        var guild = CreateGuild(0, (100, 1), (101, 1));
        var user1 = CreateUser(1, 100);
        var user2 = CreateUser(2, 101);

        Compare(guild, user1, user2, true);
    }

    [TestMethod]
    public void EqualRolesCompare()
    {
        var guild = CreateGuild(0, (100, 1));
        var user1 = CreateUser(1, 100);
        var user2 = CreateUser(2, 100);

        Equal(guild, user1, user2);
    }
}
