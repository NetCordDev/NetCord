using NetCord;

namespace RoleTest;

[TestClass]
public sealed class RoleComparison
{
    private static Role Create(ulong id, int position)
    {
        return new(new()
        {
            Id = id,
            Position = position,
        }, default, null!);
    }

    private static void Compare(Role role1, Role role2, bool role1Greater)
    {
        if (role1Greater)
        {
            Assert.IsGreaterThan(0, role1.CompareTo(role2));
            Assert.IsLessThan(0, role2.CompareTo(role1));
        }
        else
        {
            Assert.IsLessThan(0, role1.CompareTo(role2));
            Assert.IsGreaterThan(0, role2.CompareTo(role1));
        }

        Assert.AreEqual(role1Greater, role1 > role2);
        Assert.AreEqual(!role1Greater, role2 > role1);

        Assert.AreEqual(role1Greater, role1 >= role2);
        Assert.AreEqual(!role1Greater, role2 >= role1);

        Assert.AreEqual(!role1Greater, role1 < role2);
        Assert.AreEqual(role1Greater, role2 < role1);

        Assert.AreEqual(!role1Greater, role1 <= role2);
        Assert.AreEqual(role1Greater, role2 <= role1);
    }

    private static void Equal(Role role1, Role role2)
    {
        Assert.AreEqual(0, role1.CompareTo(role2));
        Assert.AreEqual(0, role2.CompareTo(role1));

        Assert.IsFalse(role1 > role2);
        Assert.IsFalse(role2 > role1);

        Assert.IsTrue(role1 >= role2);
        Assert.IsTrue(role2 >= role1);

        Assert.IsFalse(role1 < role2);
        Assert.IsFalse(role2 < role1);

        Assert.IsTrue(role1 <= role2);
        Assert.IsTrue(role2 <= role1);
    }

    [TestMethod]
    public void PositionCompare1()
    {
        var role1 = Create(1, 2);
        var role2 = Create(1, 1);

        Compare(role1, role2, true);
        Compare(role2, role1, false);
    }

    [TestMethod]
    public void PositionCompare2()
    {
        var role1 = Create(1, 2);
        var role2 = Create(2, 1);

        Compare(role1, role2, true);
        Compare(role2, role1, false);
    }

    [TestMethod]
    public void PositionCompare3()
    {
        var role1 = Create(2, 2);
        var role2 = Create(1, 1);

        Compare(role1, role2, true);
        Compare(role2, role1, false);
    }

    [TestMethod]
    public void SamePositionCompare()
    {
        var role1 = Create(1, 1);
        var role2 = Create(2, 1);

        Compare(role1, role2, true);
        Compare(role2, role1, false);
    }

    [TestMethod]
    public void Equal()
    {
        var role1 = Create(1, 1);
        var role2 = Create(1, 1);

        Equal(role1, role2);
        Equal(role2, role1);
    }
}
