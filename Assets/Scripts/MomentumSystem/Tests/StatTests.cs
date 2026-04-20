using NUnit.Framework;
using RPG.StatSystem;

[TestFixture]
public class StatTests
{
    [Test]
    public void ValueStat_CalculatesCurrentCorrectly()
    {
        var strength = new ValueStat("Strength", 40);
        strength.Buff = 15;
        strength.Debuff = 5;

        Assert.AreEqual(50, strength.Current);
    }

    [Test]
    public void ResourceStat_Bound_ClampsCorrectly()
    {
        var health = new ResourceStat("Health", 100, ResourceBinding.Bound);

        health.Current = 150;   // Over max
        Assert.AreEqual(100, health.Current);

        health.Current = -20;   // Below min
        Assert.AreEqual(0, health.Current);
    }

    [Test]
    public void ResourceStat_Unbound_AllowsNegativeValues()
    {
        var momentum = new ResourceStat("Momentum", 5, ResourceBinding.Unbound);
        momentum.Current = -8;
        Assert.AreEqual(-8, momentum.Current);
    }
}