using NUnit.Framework;
using RPG.StatSystem;
using RPG.AbilitySystem;
using UnityEngine;

[TestFixture]
public class StatSheetTests
{
    private StatSheet CreateTestSheet(string name = "TestCharacter")
    {
        var sheet = new StatSheet();
        sheet.characterName = name;
        return sheet;
    }

    #region Basic Initialization & Stats

    [Test]
    public void Constructor_InitializesAllStatsCorrectly()
    {
        var sheet = CreateTestSheet();

        Assert.AreEqual(100, sheet.health.Current);
        Assert.AreEqual(10, sheet.momentum.Current);
        Assert.AreEqual(10, sheet.motive.Current);
        Assert.AreEqual(10, sheet.means.Current);
        Assert.AreEqual(3, sheet.skill.Current);
        Assert.IsTrue(sheet.isAlive());
    }

    #endregion

    #region Health & Damage

    [Test]
    public void TakesDamage_ReducesHealth()
    {
        var sheet = CreateTestSheet();
        int startHealth = sheet.health.Current;

        sheet.TakesDamage(30);
        Assert.AreEqual(startHealth - 30, sheet.health.Current);
    }

    [Test]
    public void Heals_IncreasesHealth()
    {
        var sheet = CreateTestSheet();
        sheet.TakesDamage(40);
        int damagedHealth = sheet.health.Current;

        sheet.Heals(15);
        Assert.AreEqual(damagedHealth + 15, sheet.health.Current);
    }

    [Test]
    public void isAlive_ReturnsFalse_WhenHealthIsZeroOrBelow()
    {
        var sheet = CreateTestSheet();
        sheet.TakesDamage(150);
        Assert.IsFalse(sheet.isAlive());
    }

    #endregion

    #region Defensive Layers - Wards

    [Test]
    public void Ward_BlocksEntireAttack()
    {
        var sheet = CreateTestSheet();
        sheet.AddWard(1);

        int damageThrough = sheet.ApplyDefenses(45);

        Assert.AreEqual(0, damageThrough);
        Assert.AreEqual(0, sheet.wards);
    }

    [Test]
    public void MultipleWards_BlockMultipleAttacks()
    {
        var sheet = CreateTestSheet();
        sheet.AddWard(2);

        Assert.AreEqual(0, sheet.ApplyDefenses(30));
        Assert.AreEqual(1, sheet.wards);

        Assert.AreEqual(0, sheet.ApplyDefenses(50));
        Assert.AreEqual(0, sheet.wards);
    }

    #endregion

    #region Defensive Layers - Shields

    [Test]
    public void Shield_AbsorbsUpToItsAmount()
    {
        var sheet = CreateTestSheet();
        sheet.AddShield(25);

        Assert.AreEqual(5, sheet.ApplyDefenses(30));
        Assert.AreEqual(0, sheet.shield);
    }

    [Test]
    public void Shield_FullAbsorption_ReturnsZero()
    {
        var sheet = CreateTestSheet();
        sheet.AddShield(40);

        Assert.AreEqual(0, sheet.ApplyDefenses(30));
        Assert.AreEqual(10, sheet.shield);
    }

    #endregion

    #region Grit Interaction

    [Test]
    public void Grit_ReducesDamage_ButNeverBelow1()
    {
        var sheet = CreateTestSheet();
        var grit = ScriptableObject.CreateInstance<Grit>();
        grit.Initialize(15, true, 1, 1); //Onhit, lasts 1 turn and 1 round
        sheet.AbilityHit(grit);                    // Add grit as continuing effect

        Assert.AreEqual(10, sheet.ApplyDefenses(25)); // 25 - 15 = 10
        Assert.AreEqual(1, sheet.ApplyDefenses(10)); // clamped to 1
    }

    #endregion

    #region Momentum

    [Test]
    public void RefreshMomentum_AddsMotiveToCurrent()
    {
        var sheet = CreateTestSheet();
        sheet.momentum.Current = 2;

        sheet.RefreshMomentum();

        Assert.AreEqual(12, sheet.momentum.Current); // 2 + 10 motive
    }

    [Test]
    public void SpendMomentum_ReducesMomentum()
    {
        var sheet = CreateTestSheet();
        sheet.SpendMomentum(4);
        Assert.AreEqual(6, sheet.momentum.Current);
    }

    #endregion
}