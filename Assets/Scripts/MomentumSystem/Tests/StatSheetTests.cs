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

    #region Continuing Effects Management

    [Test]
    public void AbilityHit_AddsContinuingBehavior_WhenItContinues()
    {
        var sheet = CreateTestSheet();
        var damageOverTime = CreateBehavior<Damage>();
        damageOverTime.Initialize(10, false, 3, 0); // 3 rounds

        sheet.AbilityHit(damageOverTime);

        Assert.AreEqual(1, sheet.continuingEffects.Count);
        Assert.IsTrue(sheet.continuingEffects.Contains(damageOverTime));
    }

    [Test]
    public void AbilityHit_DoesNotAddBehavior_ThatDoesNotContinue()
    {
        var sheet = CreateTestSheet();
        var oneShot = CreateBehavior<Damage>();
        oneShot.Initialize(15, true, 0, 0); // onHit only, no rounds/turns

        sheet.AbilityHit(oneShot);

        Assert.AreEqual(0, sheet.continuingEffects.Count);
    }

    [Test]
    public void ApplyRoundEffects_RemovesFinishedBehaviors()
    {
        var sheet = CreateTestSheet();
        var behavior = CreateBehavior<Damage>();
        behavior.Initialize(5, false, 1, 0); // lasts 1 round

        sheet.AbilityHit(behavior);
        Assert.AreEqual(1, sheet.continuingEffects.Count);

        // Apply one round; should finish and be removed
        sheet.ApplyRoundEffects();

        Assert.AreEqual(0, sheet.continuingEffects.Count);
    }

    [Test]
    public void ApplyTurnEffects_RemovesFinishedBehaviors()
    {
        var sheet = CreateTestSheet();
        var behavior = CreateBehavior<Damage>();
        behavior.Initialize(8, false, 0, 2); // lasts 2 turns

        sheet.AbilityHit(behavior);

        sheet.ApplyTurnEffects();
        Assert.AreEqual(1, sheet.continuingEffects.Count); // still 1 turn left

        sheet.ApplyTurnEffects();
        Assert.AreEqual(0, sheet.continuingEffects.Count); // now finished
    }

    [Test]
    public void ApplyRoundEffects_And_ApplyTurnEffects_CanBothRun_OnSameBehavior()
    {
        var sheet = CreateTestSheet();
        var behavior = CreateBehavior<Damage>();
        behavior.Initialize(10, false, 1, 1); // 1 round + 1 turn

        sheet.AbilityHit(behavior);

        sheet.ApplyRoundEffects(); // consumes round
        Assert.AreEqual(1, sheet.continuingEffects.Count);

        sheet.ApplyTurnEffects(); // consumes turn; should be removed
        Assert.AreEqual(0, sheet.continuingEffects.Count);
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

    #region Helper Methods
    
    private T CreateBehavior<T>() where T : ABehavior => ScriptableObject.CreateInstance<T>();

    #endregion
}