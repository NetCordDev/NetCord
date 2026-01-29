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
            Assert.IsGreaterThan(0, role1.Position.CompareTo(role2.Position));
            Assert.IsLessThan(0, role2.Position.CompareTo(role1.Position));
        }
        else
        {
            Assert.IsLessThan(0, role1.Position.CompareTo(role2.Position));
            Assert.IsGreaterThan(0, role2.Position.CompareTo(role1.Position));
        }

        Assert.IsFalse(role1.Position == role2.Position);
        Assert.IsFalse(role2.Position == role1.Position);

        Assert.IsTrue(role1.Position != role2.Position);
        Assert.IsTrue(role2.Position != role1.Position);

        Assert.AreEqual(role1Greater, role1.Position > role2.Position);
        Assert.AreEqual(!role1Greater, role2.Position > role1.Position);

        Assert.AreEqual(role1Greater, role1.Position >= role2.Position);
        Assert.AreEqual(!role1Greater, role2.Position >= role1.Position);

        Assert.AreEqual(!role1Greater, role1.Position < role2.Position);
        Assert.AreEqual(role1Greater, role2.Position < role1.Position);

        Assert.AreEqual(!role1Greater, role1.Position <= role2.Position);
        Assert.AreEqual(role1Greater, role2.Position <= role1.Position);
    }

    private static void Equal(Role role1, Role role2)
    {
        Assert.AreEqual(0, role1.Position.CompareTo(role2.Position));
        Assert.AreEqual(0, role2.Position.CompareTo(role1.Position));

        Assert.IsTrue(role1.Position == role2.Position);
        Assert.IsTrue(role2.Position == role1.Position);

        Assert.IsFalse(role1.Position != role2.Position);
        Assert.IsFalse(role2.Position != role1.Position);

        Assert.IsFalse(role1.Position > role2.Position);
        Assert.IsFalse(role2.Position > role1.Position);

        Assert.IsTrue(role1.Position >= role2.Position);
        Assert.IsTrue(role2.Position >= role1.Position);

        Assert.IsFalse(role1.Position < role2.Position);
        Assert.IsFalse(role2.Position < role1.Position);

        Assert.IsTrue(role1.Position <= role2.Position);
        Assert.IsTrue(role2.Position <= role1.Position);
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
