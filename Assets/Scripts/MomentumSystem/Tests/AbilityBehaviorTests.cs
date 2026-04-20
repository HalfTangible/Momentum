using NUnit.Framework;
using RPG.AbilitySystem;
using RPG.StatSystem;
using UnityEngine;

[TestFixture]
public class AbilityBehaviorTests
{
    private T Create<T>() where T : ABehavior => ScriptableObject.CreateInstance<T>();
    
 private StatSheet CreateStatSheet(string name = "TestTarget")
    {
        var sheet = new StatSheet();
        sheet.characterName = name;
        return sheet;
    }

    #region Damage Tests

    [Test]
    public void Damage_Initialize_SetsValues()
    {
        var damage = Create<Damage>();
        damage.Initialize(35, true);

        Assert.AreEqual(35, (int)damage.GetStat<int>("AMOUNT"));
        Assert.IsTrue((bool)damage.GetStat<bool>("ONHIT"));
    }

    #endregion

    #region Ward Tests

    [Test]
    public void Ward_OnHit_AddsWard_And_BlocksNextAttack()
    {
        var ward = Create<Ward>();
        ward.Initialize(1);                    // 1 ward charge

        var target = CreateStatSheet();
        ward.Affects(target);

        Assert.AreEqual(1, target.wards);

        // First attack should be completely blocked
        int damageAfter = target.ApplyDefenses(45);
        Assert.AreEqual(0, damageAfter);
        Assert.AreEqual(0, target.wards);      // Ward consumed
    }

    [Test]
    public void Ward_MultipleCharges_BlockMultipleAttacks()
    {
        var ward = Create<Ward>();
        ward.Initialize(2);                    // 2 charges

        var target = CreateStatSheet();
        ward.Affects(target);

        Assert.AreEqual(2, target.wards);

        Assert.AreEqual(0, target.ApplyDefenses(30));
        Assert.AreEqual(1, target.wards);

        Assert.AreEqual(0, target.ApplyDefenses(50));
        Assert.AreEqual(0, target.wards);
    }

    #endregion

    #region Grit Tests

    [Test]
    public void Grit_ModifyIncomingDamage_ReducesDamage()
    {
        var grit = Create<Grit>();
        grit.Initialize(20);

        Assert.AreEqual(30, grit.ModifyIncomingDamage(50));
        Assert.AreEqual(1, grit.ModifyIncomingDamage(15));
        Assert.AreEqual(1, grit.ModifyIncomingDamage(10));
    }

    #endregion

    #region ABehavior Tests

    [Test]
    public void ABehavior_Continues_ReturnsCorrectValue()
    {
        var behavior = Create<Damage>();

        behavior.SetStat("ROUNDS", 2);
        Assert.IsTrue(behavior.Continues());

        behavior.SetStat("ROUNDS", 0);
        behavior.SetStat("TURNS", 0);
        Assert.IsFalse(behavior.Continues());
    }

    #endregion

    #region Shield Tests

    [Test]
    public void Shield_AbsorbsDamage_UpToItsAmount()
    {
        var target = CreateStatSheet();
        target.AddShield(25);
        int startHealth = target.health.Current;

        int damageAfterDefenses = target.ApplyDefenses(30);

        target.TakesDamage(damageAfterDefenses);

        // 30 incoming damage with 25 shield; 5 gets through
        Assert.AreEqual(5, damageAfterDefenses);
        Assert.AreEqual(0, target.shield);           // Shield is fully consumed
        Assert.AreEqual(startHealth - damageAfterDefenses, target.health.Current);

    }

    [Test]
    public void Shield_PartialAbsorption_LeavesRemainingDamage()
    {
        var target = CreateStatSheet();
        int shieldAmount = 10;
        int rawDamage = 35;
        int startHealth = target.health.Current;
        target.AddShield(shieldAmount);
        int damageDealt = target.ApplyDefenses(rawDamage); // 35 - 10 shield = 25 damage through
        int expectedHealth = startHealth - damageDealt;

        target.TakesDamage(damageDealt);

        Assert.AreEqual(rawDamage - shieldAmount, damageDealt); // 35 - 10 shield = 25 damage through
        Assert.AreEqual(0, target.shield);
        Assert.AreEqual(target.health.Current, expectedHealth);

    }

    [Test]
    public void Shield_FullAbsorption_NoDamageTaken()
    {
        var target = CreateStatSheet();
        int startShield = 40;
        target.AddShield(startShield);
        int startHealth = target.health.Current;
        int rawDamage = 30;

        int damageAfter = target.ApplyDefenses(rawDamage);
        int remainingShield = startShield - rawDamage;

        Assert.AreEqual(0, damageAfter);
        Assert.AreEqual(remainingShield, target.shield);        // 10 shield remaining

        target.TakesDamage(damageAfter);
        Assert.AreEqual(startHealth, target.health.Current);   // No damage taken
    }

    #endregion
}