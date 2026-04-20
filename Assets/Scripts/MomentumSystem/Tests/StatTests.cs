using NUnit.Framework;
using RPG.StatSystem;

[TestFixture]
public class StatTests
{
    [Test]
    public void ValueStat_CalculatesCurrentCorrectly()
    {
        int baseStrength = 40;
        int buffValue = 15;
        int debuffValue = 5;

        var strength = new ValueStat("Strength", baseStrength);
        Assert.AreEqual(baseStrength, strength.Current);

        strength.Buff = 15;
        Assert.AreEqual(baseStrength + buffValue, strength.Current);
        
        strength.Debuff = 5;
        Assert.AreEqual(baseStrength + buffValue - debuffValue, strength.Current);

        Assert.AreEqual(baseStrength, strength.Base);

        //Check base
    }

    [Test]
    public void ValueStat_Minimum_One()
    {
        int baseValue = 5;
        int debuffValue = 10;

        var motive = new ValueStat("Motive", baseValue);

        motive.Debuff = debuffValue;

        Assert.AreEqual(1, motive.Current);
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