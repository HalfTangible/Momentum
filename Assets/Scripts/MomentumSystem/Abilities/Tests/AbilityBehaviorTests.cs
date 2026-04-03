using NUnit.Framework;
using RPG.AbilitySystem;
using UnityEngine;

[TestFixture]
public class AbilityBehaviorTests
{
    private T Create<T>() where T : ABehavior => ScriptableObject.CreateInstance<T>();

    [Test]
    public void ABehavior_SetAndGetStat_WorksForAmount()
    {
        var dmg = Create<Damage>();
        dmg.SetStat("AMOUNT", 42);
        Assert.AreEqual(42, (int)dmg.GetStat<int>("AMOUNT"));
    }

    [Test]
    public void Damage_Initialize_SetsValuesCorrectly()
    {
        var damage = Create<Damage>();
        damage.Initialize(25, true);   // amount, onHit

        Assert.AreEqual(25, (int)damage.GetStat<int>("AMOUNT"));
        Assert.IsTrue((bool)damage.GetStat<bool>("ONHIT"));
    }

    [Test]
    public void Grit_ModifyIncomingDamage_ReducesCorrectly()
    {
        var grit = Create<Grit>();
        grit.Initialize(15);

        Assert.AreEqual(35, grit.ModifyIncomingDamage(50));
        Assert.AreEqual(0, grit.ModifyIncomingDamage(10));   // doesn't go negative
    }

    [Test]
    public void ABehavior_Continues_LogicWorks()
    {
        var behavior = Create<Damage>();
        behavior.SetStat("ROUNDS", 1);
        Assert.IsTrue(behavior.Continues());

        behavior.SetStat("ROUNDS", 0);
        behavior.SetStat("TURNS", 0);
        Assert.IsFalse(behavior.Continues());
    }
}