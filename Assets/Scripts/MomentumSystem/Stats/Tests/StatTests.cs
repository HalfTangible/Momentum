using NUnit.Framework;
using RPG.StatSystem;

[TestFixture]
public class StatTests
{
    #region ValueStat Tests

    [Test]
    public void ValueStat_Constructor_SetsCorrectValues()
    {
        var stat = new ValueStat("Strength", 45);

        Assert.AreEqual("Strength", stat.Name);
        Assert.AreEqual(45, stat.Base);
        Assert.AreEqual(45, stat.Current);
        Assert.AreEqual(1, stat.Min);
    }

    [Test]
    public void ValueStat_Current_IncludesBuffAndDebuff()
    {
        var stat = new ValueStat("Agility", 30);
        stat.Buff = 15;
        stat.Debuff = 5;

        Assert.AreEqual(40, stat.Current);
    }

    #endregion

    #region ResourceStat - Bound (Health, Stamina, etc.)

    [Test]
    public void ResourceStat_Bound_InitializesCorrectly()
    {
        var health = new ResourceStat("Health", 120, ResourceBinding.Bound);

        Assert.AreEqual(120, health.Current);
        Assert.AreEqual(120, health.Remaining);
        Assert.AreEqual(120, health.Max);
        Assert.AreEqual(0, health.Min);
    }

    [Test]
    public void ResourceStat_Bound_ClampsToZeroAndMax()
    {
        var health = new ResourceStat("Health", 100, ResourceBinding.Bound);

        health.Current = 150;   // Over max
        Assert.AreEqual(100, health.Current);

        health.Current = -50;   // Below min
        Assert.AreEqual(0, health.Current);
    }

    #endregion

    #region ResourceStat - Unbound (Momentum, Rage, etc.)

    [Test]
    public void ResourceStat_Unbound_InitializesCorrectly()
    {
        var momentum = new ResourceStat("Momentum", 5, ResourceBinding.Unbound);

        Assert.AreEqual(5, momentum.Current);
        Assert.AreEqual(5, momentum.Remaining);
        Assert.AreEqual(-1, momentum.Min);   // Your original sentinel value
        Assert.AreEqual(-1, momentum.Max);
    }

    [Test]
    public void ResourceStat_Unbound_AllowsNegativeAndHighValues()
    {
        var momentum = new ResourceStat("Momentum", 5, ResourceBinding.Unbound);

        momentum.Current = -4;
        Assert.AreEqual(-4, momentum.Current);

        momentum.Current = -999;
        Assert.AreEqual(-999, momentum.Current);

        momentum.Current = 500;
        Assert.AreEqual(500, momentum.Current);
    }

    #endregion

    #region Buff / Debuff Interaction

    [Test]
    public void ResourceStat_Bound_ClampsAfterBuffDebuffApplied()
    {
        var health = new ResourceStat("Health", 100, ResourceBinding.Bound);
        health.Buff = 20;
        health.Debuff = 150;

        Assert.AreEqual(0, health.Current); // Should clamp to 0
    }

    #endregion
}