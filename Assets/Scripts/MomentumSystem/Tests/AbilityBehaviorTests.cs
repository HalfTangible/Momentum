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

        target.AbilityHit(damage);

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

        target.AbilityHit(damage);

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

        target.AbilityHit(damage);

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
        int damageAmount = 10;
        int turnDuration = 1;
        int roundDuration = 1;
        bool onHit = false;
        var damage = Create<Damage>();
        damage.Initialize(damageAmount, onHit, turnDuration, roundDuration);

        var target = CreateStatSheet();
        int startHealth = target.health.Current;


        Debug.Log("Starting health: " + target.health.Current);

        target.AbilityHit(damage);

        Debug.Log("Before damage health 1: " + target.health.Current);
        target.ApplyRoundEffects();
        Assert.AreEqual(startHealth - 10, target.health.Current);
        Debug.Log("After damage health 1: " + target.health.Current);

        target.ApplyTurnEffects();
        Debug.Log("Before damage health 2: " + target.health.Current);
        Assert.AreEqual(startHealth - 20, target.health.Current);
        Debug.Log("After damage health 2: " + target.health.Current);
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
        shield.Initialize(25, true);                    // 25 shield points

        var target = CreateStatSheet();
        target.AbilityHit(shield);

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
        int rounds = 2;
        int turns = 0;
        shield.Initialize(15, false, rounds, turns);     // 2 rounds, not on hit

        var target = CreateStatSheet();
        Debug.Log("Shield 0: " + target.shield);
        target.AbilityHit(shield);                 // Initial application
        target.ApplyRoundEffects();
        Assert.AreEqual(15, target.shield);
        Debug.Log("Shield 1: " + target.shield);

        target.ApplyRoundEffects();
        Debug.Log("Shield 2: " + target.shield);
        Assert.AreEqual(30, target.shield);     // Second application
        Debug.Log("Shield 3: " + target.shield);

        target.ApplyRoundEffects();
        Debug.Log("Shield 4: " + target.shield);
        Assert.AreEqual(30, target.shield);     // Should not apply more
        Debug.Log("Shield 5: " + target.shield);
    }

    [Test]
    public void Shield_WithTurns_AppliesMultipleTimes()
    {
        var shield = Create<Shield>();
        shield.Initialize(10, false, 0, 3);     // 3 turns

        var target = CreateStatSheet();
        target.AbilityHit(shield);
        Assert.AreEqual(0, target.shield);

        target.ApplyTurnEffects();
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
        var healing = Create<Healing>();  // Heal 30
        healing.Initialize(30);
        var damage = Create<Damage>();
        damage.Initialize(40);

        var target = CreateStatSheet();
        target.AbilityHit(damage);
        int startHealth = target.health.Current;

        healing.Affects(target);

        Assert.AreEqual(startHealth + 30, target.health.Current);
    }

    [Test]
    public void Healing_WithRounds_HealsEachRound()
    {
        var damage = Create<Damage>();
        damage.Initialize(45, true, 0, 0);

        var healing = Create<Healing>();
        healing.Initialize(15, false, 3, 0);       // 3 rounds, not on hit

        var target = CreateStatSheet();
        Debug.Log("Before damage 1: " + target.health.Current + "/" + target.health.Max);
        target.AbilityHit(damage);
        Debug.Log("After damage 1: " + target.health.Current + "/" + target.health.Max);

        int startHealth = target.health.Current;

        Debug.Log("Before heal 1: " + target.health.Current + "/" + target.health.Max);

        target.AbilityHit(healing);                   // Initial heal?

        Debug.Log("Before heal 2: " + target.health.Current + "/" + target.health.Max);

        target.ApplyRoundEffects();
        Debug.Log("After heal 2: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 15, target.health.Current);

        Debug.Log("Before heal 3: " + target.health.Current + "/" + target.health.Max);

        target.ApplyRoundEffects();
        Debug.Log("After heal 3: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 30, target.health.Current);

        Debug.Log("Before heal 4: " + target.health.Current + "/" + target.health.Max);

        target.ApplyRoundEffects();
        Debug.Log("After heal 4: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 45, target.health.Current);

        Debug.Log("Before heal 5: " + target.health.Current + "/" + target.health.Max);
    }

    [Test]
    public void Healing_WithTurns_HealsEachTurn()
    {
        var healing = Create<Healing>();
        healing.Initialize(10, false, 0, 4);       // 4 turns

        var target = CreateStatSheet();

        var damage = Create<Damage>();
        damage.Initialize(45, true, 0, 0);

        Debug.Log("Before damage 1: " + target.health.Current + "/" + target.health.Max);
        target.AbilityHit(damage);
        Debug.Log("After damage 1: " + target.health.Current + "/" + target.health.Max);

        int startHealth = target.health.Current;

        Debug.Log("Before heal 1: " + target.health.Current + "/" + target.health.Max);
        target.AbilityHit(healing);
        Debug.Log("After heal 1: " + target.health.Current + "/" + target.health.Max);

        Debug.Log("Before heal 2: " + target.health.Current + "/" + target.health.Max);
        target.ApplyTurnEffects();
        Debug.Log("After heal 2: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 10, target.health.Current);

        Debug.Log("Before heal 3: " + target.health.Current + "/" + target.health.Max);
        target.ApplyTurnEffects();
        Debug.Log("After heal 3: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 20, target.health.Current);

        Debug.Log("Before heal 4: " + target.health.Current + "/" + target.health.Max);
        target.ApplyTurnEffects();
        Debug.Log("After heal 4: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 30, target.health.Current);

        Debug.Log("Before heal 5: " + target.health.Current + "/" + target.health.Max);
        target.ApplyTurnEffects();
        Debug.Log("After heal 5: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 40, target.health.Current);
        Assert.AreEqual(0, target.continuingEffects.Count); // should be cleaned up

        Debug.Log("Before heal 6: " + target.health.Current + "/" + target.health.Max);
        target.ApplyTurnEffects();
        Debug.Log("After heal 6: " + target.health.Current + "/" + target.health.Max);
        Assert.AreEqual(startHealth + 40, target.health.Current);
    }

    #endregion

    #region Buff Tests

    [Test]
    public void Buff_OnHit_BuffsCorrectStat()
    {
        var buff = Create<Buff>();
        buff.Initialize(8);
        buff.setTargetStat("MOTIVE");
        
        Debug.Log("targetStat: " + buff.getTargetStat());

        var target = CreateStatSheet();
        int startMotive = target.motive.Current;

        buff.Affects(target);

        Assert.AreEqual(startMotive + 8, target.motive.Current);
    }

    [Test]
    public void Buff_AsContinuingEffect_BuffsAndLaterReversesCorrectly()
    {
        int buffAmount = 12;
        int rounds = 0;
        int turns = 2;
        bool onHit = true; //A buff or debuff by default should be an onHit effect, else it would do nothing of substance
        var buff = Create<Buff>();
        buff.Initialize(buffAmount, onHit, rounds, turns);           // 2 turns
        buff.setTargetStat("SKILL");

        var target = CreateStatSheet();
        int startSkill = target.skill.Current;

        Debug.Log("Skill 0: " + target.skill.Current);
        Assert.AreEqual(startSkill, target.skill.Current);

        target.AbilityHit(buff);         // Make it a continuing effect

        Debug.Log("Skill 1.1: " + target.skill.Current);

        target.ApplyTurnEffects(); // End turn 1
        Debug.Log("Skill 1.2: " + target.skill.Current);
        Assert.AreEqual(startSkill + buffAmount, target.skill.Current);

        Debug.Log("Skill 2: " + target.skill.Current);

        target.ApplyTurnEffects(); // End turn 2
        Assert.AreEqual(startSkill, target.skill.Current); // Buff removed

        Debug.Log("Skill 3: " + target.skill.Current);

        target.ApplyTurnEffects();
        Assert.AreEqual(startSkill, target.skill.Current); // Buff already removed.

        Debug.Log("Skill 4: " + target.skill.Current);
    }

    private Buff Skill_Buff_Test()
    {
        var buff = Create<Buff>();
        buff.Initialize(15);
        buff.setTargetStat("SKILL");
        return buff;
    }

    [Test]
    public void Buff_MultipleApplications_StackCorrectly()
    {
        var buff1 = Skill_Buff_Test();
        var buff2 = Skill_Buff_Test();
        int baseAmount = buff1.getAmount();
        Debug.Log("buff1: " + buff1.getAmount());
        Debug.Log("buff2: " + buff2.getAmount());
        Debug.Log("baseAmount: " + baseAmount);

        var target = CreateStatSheet();
        int startMotive = target.skill.Current;

        Debug.Log("startMotive: " + startMotive);

        target.AbilityHit(buff1);
        target.AbilityHit(buff2);

        Debug.Log("Current motive: " + target.skill.Current);
        Debug.Log("Should be motive: " + (startMotive + (baseAmount * 2)));

        Assert.AreEqual((startMotive + (baseAmount * 2)), target.skill.Current);
    }

    #endregion
}