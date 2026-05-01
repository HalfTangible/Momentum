using NUnit.Framework;
using RPG.AbilitySystem;
using RPG.StatSystem;
using UnityEngine;

[TestFixture]
public class AbilityBehaviorTests
{
    #region Setup & Helper Methods
    private T Create<T>() where T : ABehavior => ScriptableObject.CreateInstance<T>();
    
 private StatSheet CreateStatSheet(string name = "TestTarget")
    {
        var sheet = new StatSheet();
        sheet.characterName = name;
        return sheet;
    }

#endregion

    #region Damage Tests

    [Test]
    public void Damage_Initialize_SetsValues()
    {
        var damage = Create<Damage>();
        damage.Initialize(35, true);

        Assert.AreEqual(35, (int)damage.GetStat<int>("AMOUNT"));
        Assert.IsTrue((bool)damage.GetStat<bool>("ONHIT"));
    }

    [Test]
    public void Damage_OnHit_DealsImmediateDamage()
    {
        var damage = Create<Damage>();
        damage.Initialize(35, true);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        damage.Affects(target);

        Assert.AreEqual(startHealth - 35, target.health.Current);
    }

    [Test]
    public void Damage_OnHit_RespectsDefenses()
    {
        var damage = Create<Damage>();
        damage.Initialize(50, true);

        var target = CreateStatSheet();
        target.AddShield(20);

        int startHealth = target.health.Current;

        damage.Affects(target);

        Assert.AreEqual(startHealth - 30, target.health.Current);
        Assert.AreEqual(0, target.shield);
    }

    [Test]
    public void Damage_WithRounds_DealsDamageEachRound()
    {
        var damage = Create<Damage>();
        damage.Initialize(12, false, 3, 0);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        damage.Affects(target);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth - 12, target.health.Current);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth - 24, target.health.Current);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth - 36, target.health.Current);
    }

    [Test]
    public void Damage_WithTurns_DealsDamageEachTurn()
    {
        var damage = Create<Damage>();
        damage.Initialize(8, false, 0, 4);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        damage.Affects(target);
        target.continuingEffects.Add(damage);

        for (int i = 0; i < 4; i++)
        {
            target.ApplyTurnEffects();
        }

        Assert.AreEqual(startHealth - 32, target.health.Current);
        Assert.AreEqual(0, target.continuingEffects.Count);
    }

    [Test]
    public void Damage_WithBothRoundsAndTurns_DealsDamageOnBoth()
    {
        var damage = Create<Damage>();
        damage.Initialize(10, false, 1, 1);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        damage.Affects(target);
        target.continuingEffects.Add(damage);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth - 10, target.health.Current);

        target.ApplyTurnEffects();
        Assert.AreEqual(startHealth - 20, target.health.Current);
        Assert.AreEqual(0, target.continuingEffects.Count);
    }

    [Test]
    public void Damage_Overwhelms_DealsDamageImmediately()
    {
        var damage = Create<Damage>();
        damage.Initialize(45);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        damage.Overwhelms(target);

        Assert.AreEqual(startHealth - 45, target.health.Current);
    }

    [Test]
    public void Damage_RespectsGritReduction()
    {
        var damage = Create<Damage>();
        damage.Initialize(40, true);

        var target = CreateStatSheet();
        var grit = Create<Grit>();
        grit.Initialize(15);
        target.continuingEffects.Add(grit);

        int startHealth = target.health.Current;

        damage.Affects(target);

        Assert.AreEqual(startHealth - 25, target.health.Current);
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

    [Test]
    public void Ward_WithRounds_LastMultipleRounds()
    {
        var ward = Create<Ward>();
        ward.Initialize(1, false, 2, 0);    // 2 rounds, not on hit

        var target = CreateStatSheet();
        ward.Affects(target);               // Initial application

        Assert.AreEqual(1, target.wards);

        target.ApplyRoundEffects();
        Assert.AreEqual(1, target.wards);   // Still has 1 round left

    }

    #endregion

    #region Grit Tests

    [Test]
    public void Grit_ModifyIncomingDamage_ReducesDamage_And_ClampsToMinimum1()
    {
        var grit = Create<Grit>();
        grit.Initialize(20);        // reduces by 20

        Assert.AreEqual(30, grit.ModifyIncomingDamage(50));   // 50 - 20 = 30
        Assert.AreEqual(1, grit.ModifyIncomingDamage(15));   // 15 - 20, clamped to 1
        Assert.AreEqual(1, grit.ModifyIncomingDamage(10));
        Assert.AreEqual(1, grit.ModifyIncomingDamage(1));    // Always at least 1 damage
        Assert.AreEqual(1, grit.ModifyIncomingDamage(0));    // Edge case
    }

    [Test]
    public void Grit_AsContinuingEffect_ReducesDamageOnEveryHit()
    {
        var grit = Create<Grit>();
        grit.Initialize(12, false, 0, 5);        // lasts 5 turns

        var target = CreateStatSheet();
        target.continuingEffects.Add(grit);      // Add as active effect

        Assert.AreEqual(18, target.ApplyDefenses(30));   // 30 - 12
        Assert.AreEqual(1, target.ApplyDefenses(8));    // clamped
        Assert.AreEqual(1, target.ApplyDefenses(1));
    }

    [Test]
    public void Grit_OnlyReducesDamage_WhenInContinuingEffects()
    {
        var grit = Create<Grit>();
        grit.Initialize(25);

        var target = CreateStatSheet();

        // Not in continuingEffects
        Assert.AreEqual(40, target.ApplyDefenses(40));   // No reduction

        // Now add it
        target.continuingEffects.Add(grit);
        Assert.AreEqual(15, target.ApplyDefenses(40));   // Now reduced
    }

    [Test]
    public void Grit_WithRounds_AppliesReductionEachRound()
    {
        var grit = Create<Grit>();
        grit.Initialize(8, false, 2, 0);         // 2 rounds

        var target = CreateStatSheet();
        target.continuingEffects.Add(grit);

        Assert.AreEqual(22, target.ApplyDefenses(30));   // 30 - 8

        target.ApplyRoundEffects();
        Assert.AreEqual(22, target.ApplyDefenses(30));   // Still active

        target.ApplyRoundEffects();
        Assert.AreEqual(30, target.ApplyDefenses(30));   // Duration expired
    }

    [Test]
    public void Grit_WithTurns_AppliesReductionEachTurn()
    {
        var grit = Create<Grit>();
        grit.Initialize(10, false, 0, 3);        // 3 turns

        var target = CreateStatSheet();
        target.continuingEffects.Add(grit);

        Assert.AreEqual(20, target.ApplyDefenses(30));

        target.ApplyTurnEffects();
        Assert.AreEqual(20, target.ApplyDefenses(30));

        target.ApplyTurnEffects();
        Assert.AreEqual(20, target.ApplyDefenses(30));

        target.ApplyTurnEffects();
        Assert.AreEqual(30, target.ApplyDefenses(30));   // No longer active
    }

    [Test]
    public void Grit_Overwhelms_AlsoAppliesReduction()
    {
        var grit = Create<Grit>();
        grit.Initialize(18);

        Assert.AreEqual(32, grit.ModifyIncomingDamage(50));
        Assert.AreEqual(1, grit.ModifyIncomingDamage(10));
    }

    #endregion

    #region Shield Tests

    [Test]
    public void Shield_OnHit_GrantsShieldPoints()
    {
        var shield = Create<Shield>();
        shield.Initialize(25);                    // 25 shield points

        var target = CreateStatSheet();
        shield.Affects(target);

        Assert.AreEqual(25, target.shield);
    }

    [Test]
    public void Shield_AbsorbsDamage_UpToItsAmount()
    {
        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        target.AddShield(25);

        int damageAfter = target.ApplyDefenses(30);

        Assert.AreEqual(5, damageAfter);           // 30 - 25 = 5 through
        Assert.AreEqual(0, target.shield);

        target.TakesDamage(damageAfter);
        Assert.AreEqual(startHealth - 5, target.health.Current);
    }

    [Test]
    public void Shield_FullAbsorption_NoDamageTaken()
    {
        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        target.AddShield(40);

        int damageAfter = target.ApplyDefenses(30);

        Assert.AreEqual(0, damageAfter);
        Assert.AreEqual(10, target.shield);        // 10 shield remaining

        target.TakesDamage(damageAfter);
        Assert.AreEqual(startHealth, target.health.Current);
    }

    [Test]
    public void Shield_WithRounds_AppliesMultipleTimes()
    {
        var shield = Create<Shield>();
        shield.Initialize(15, false, 2, 0);     // 2 rounds, not on hit

        var target = CreateStatSheet();
        shield.Affects(target);                 // Initial application

        Assert.AreEqual(15, target.shield);

        target.ApplyRoundEffects();
        Assert.AreEqual(30, target.shield);     // Second application

        target.ApplyRoundEffects();
        Assert.AreEqual(30, target.shield);     // Should not apply more
    }

    [Test]
    public void Shield_WithTurns_AppliesMultipleTimes()
    {
        var shield = Create<Shield>();
        shield.Initialize(10, false, 0, 3);     // 3 turns

        var target = CreateStatSheet();
        shield.Affects(target);                 // Initial application
        target.continuingEffects.Add(shield);   // Make sure it's tracked

        Assert.AreEqual(10, target.shield);

        target.ApplyTurnEffects();
        Assert.AreEqual(20, target.shield);

        target.ApplyTurnEffects();
        Assert.AreEqual(30, target.shield);

        target.ApplyTurnEffects();
        Assert.AreEqual(30, target.shield);     // Should not go higher
    }

    [Test]
    public void Shield_Overwhelms_AlsoGrantsShield()
    {
        var shield = Create<Shield>();
        shield.Initialize(20);

        var target = CreateStatSheet();
        shield.Overwhelms(target);

        Assert.AreEqual(20, target.shield);
    }

    #endregion

    #region Healing Tests

    [Test]
    public void Healing_OnHit_HealsTargetImmediately()
    {
        var healing = Create<Healing>();
        healing.Initialize(30);                    // Heal 30

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        healing.Affects(target);

        Assert.AreEqual(startHealth + 30, target.health.Current);
    }

    [Test]
    public void Healing_WithRounds_HealsEachRound()
    {
        var healing = Create<Healing>();
        healing.Initialize(15, false, 3, 0);       // 3 rounds, not on hit

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        healing.Affects(target);                   // Initial heal?

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth + 15, target.health.Current);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth + 30, target.health.Current);

        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth + 45, target.health.Current);
    }

    [Test]
    public void Healing_WithTurns_HealsEachTurn()
    {
        var healing = Create<Healing>();
        healing.Initialize(10, false, 0, 4);       // 4 turns

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        healing.Affects(target);
        target.continuingEffects.Add(healing);

        for (int i = 0; i < 4; i++)
        {
            target.ApplyTurnEffects();
        }

        Assert.AreEqual(startHealth + 40, target.health.Current);
        Assert.AreEqual(0, target.continuingEffects.Count); // should be cleaned up
    }

    [Test]
    public void Healing_Overwhelms_AlsoHeals()
    {
        var healing = Create<Healing>();
        healing.Initialize(25);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;

        healing.Overwhelms(target);

        Assert.AreEqual(startHealth + 25, target.health.Current);
    }

    #endregion

    #region Buff Tests

    [Test]
    public void Buff_Initialize_SetsDefaultTargetStat()
    {
        var buff = Create<Buff>();
        buff.Initialize(10);

        // Should default to a valid stat (usually Motive or similar)
        string target = (string)buff.GetStat<string>("TARGETSTAT");
        Assert.IsNotNull(target);
        Assert.IsTrue(target == "MOTIVE" || target == "MEANS" || target == "SKILL");
    }

    [Test]
    public void Buff_OnHit_BuffsCorrectStat()
    {
        var buff = Create<Buff>();
        buff.Initialize(8);

        var target = CreateStatSheet();
        int startMotive = target.motive.Current;

        buff.Affects(target);

        Assert.AreEqual(startMotive + 8, target.motive.Current);
    }

    [Test]
    public void Buff_AsContinuingEffect_BuffsAndLaterReversesCorrectly()
    {
        var buff = Create<Buff>();
        buff.Initialize(12, false, 0, 2);           // 2 turns

        var target = CreateStatSheet();
        int startSkill = target.skill.Current;

        buff.Affects(target);
        target.continuingEffects.Add(buff);         // Make it a continuing effect

        Assert.AreEqual(startSkill + 12, target.skill.Current);

        // First turn
        target.ApplyTurnEffects();
        Assert.AreEqual(startSkill + 12, target.skill.Current);

        // Second turn, should finish and remove buff
        target.ApplyTurnEffects();
        Assert.AreEqual(startSkill, target.skill.Current);   // Buff removed
    }

    [Test]
    public void Buff_Finished_ReversesExactAmountApplied()
    {
        var buff = Create<Buff>();
        buff.Initialize(15);

        var target = CreateStatSheet();
        int startMeans = target.means.Current;

        buff.Affects(target);
        Assert.AreEqual(startMeans + 15, target.means.Current);

        buff.Finished(target);
        Assert.AreEqual(startMeans, target.means.Current);
    }

    [Test]
    public void Buff_CanTargetDifferentStats()
    {
        var buff = Create<Buff>();

        buff.SetStat("TARGETSTAT", "SKILL");
        buff.Initialize(7);

        var target = CreateStatSheet();
        int startSkill = target.skill.Current;

        buff.Affects(target);
        Assert.AreEqual(startSkill + 7, target.skill.Current);
    }

    [Test]
    public void Buff_Overwhelms_AlsoAppliesBuff()
    {
        var buff = Create<Buff>();
        buff.Initialize(20);

        var target = CreateStatSheet();
        int startMotive = target.motive.Current;

        buff.Overwhelms(target);

        Assert.AreEqual(startMotive + 20, target.motive.Current);
    }

    [Test]
    public void Buff_MultipleApplications_StackCorrectly()
    {
        var buff = Create<Buff>();
        buff.Initialize(5);

        var target = CreateStatSheet();
        int startMotive = target.motive.Current;

        buff.Affects(target);
        buff.Affects(target);

        Assert.AreEqual(startMotive + 10, target.motive.Current);
    }

    #endregion
}