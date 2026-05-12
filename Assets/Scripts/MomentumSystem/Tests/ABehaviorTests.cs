using NUnit.Framework;
using RPG.AbilitySystem;
using RPG.StatSystem;
using UnityEngine;

[TestFixture]
public class ABehaviorTests
{
    private T Create<T>() where T : ABehavior => ScriptableObject.CreateInstance<T>();

    private StatSheet CreateTarget() => new StatSheet() { characterName = "TestTarget" };

    #region Core Properties & Getters

    [Test]
    public void ABehavior_Constructor_InitializesKeysCorrectly()
    {
        var behavior = Create<Damage>(); // Any concrete class works for testing base

        var keys = behavior.GetAllKeys();

        Assert.IsNotNull(keys);
        Assert.Contains("AMOUNT", keys);
        Assert.Contains("ONHIT", keys);
        Assert.Contains("ROUNDS", keys);
        Assert.Contains("TURNS", keys);
        Assert.Contains("ONUSER", keys);
    }

    [Test]
    public void GetStat_ReturnsCorrectValues()
    {
        var behavior = Create<Damage>();
        behavior.Initialize(42, true, 5, 3);

        Assert.AreEqual(42, (int)behavior.GetStat<int>("AMOUNT"));
        Assert.IsTrue((bool)behavior.GetStat<bool>("ONHIT"));
        Assert.AreEqual(5, (int)behavior.GetStat<int>("ROUNDS"));
        Assert.AreEqual(3, (int)behavior.GetStat<int>("TURNS"));
    }

    [Test]
    public void SetStat_UpdatesValuesCorrectly()
    {
        var behavior = Create<Damage>();

        behavior.SetStat("AMOUNT", 77);
        behavior.SetStat("ROUNDS", 4);
        behavior.SetStat("ONHIT", false);

        Assert.AreEqual(77, (int)behavior.GetStat<int>("AMOUNT"));
        Assert.AreEqual(4, (int)behavior.GetStat<int>("ROUNDS"));
        Assert.IsFalse((bool)behavior.GetStat<bool>("ONHIT"));
    }

    #endregion

    #region Continues & Duration Logic

    [Test]
    public void Continues_ReturnsTrue_WhenHasRoundsOrTurns()
    {
        var behavior = Create<Damage>();
        behavior.SetStat("ROUNDS", 2);

        Assert.IsTrue(behavior.Continues());

        behavior.SetStat("ROUNDS", 0);
        behavior.SetStat("TURNS", 3);
        Assert.IsTrue(behavior.Continues());
    }

    [Test]
    public void Continues_ReturnsFalse_WhenNoDurationLeft()
    {
        var behavior = Create<Damage>();
        behavior.SetStat("ROUNDS", 0);
        behavior.SetStat("TURNS", 0);

        Assert.IsFalse(behavior.Continues());
    }

    #endregion

    #region EachRound / EachTurn Default Behavior

    [Test]
    public void EachRound_Default_ReturnsContinuesResult()
    {
        var behavior = Create<Damage>();
        var target = CreateTarget();

        behavior.SetStat("ROUNDS", 2);
        Debug.Log("2 MORE LADIES PLEASE Rounds: " + behavior.getRounds());
        Assert.IsTrue(behavior.EachRound(target));

        //behavior.SetStat("ROUNDS", 1);
        Debug.Log("1 MORE LADY PLEASE Rounds: " + behavior.getRounds());
        Assert.IsFalse(behavior.EachRound(target));

        //behavior.SetStat("ROUNDS", 0);
        Debug.Log("NO MORE LADIES PLEASE Rounds: " + behavior.getRounds());
        Assert.IsTrue(behavior.getRounds() == 0);
    }

    [Test]
    public void EachTurn_Default_ReturnsContinuesResult()
    {
        var behavior = Create<Damage>();
        var target = CreateTarget();

        behavior.SetStat("TURNS", 2);
        Assert.IsTrue(behavior.EachTurn(target));

        //behavior.SetStat("TURNS", 1);
        Assert.IsFalse(behavior.EachTurn(target));

        //behavior.SetStat("TURNS", 0);
        Assert.IsTrue(behavior.getTurns() == 0);
    }

    #endregion

    #region ModifyIncomingDamage Default

    [Test]
    public void ModifyIncomingDamage_Default_PassesDamageThrough()
    {
        var behavior = Create<Damage>();

        Assert.AreEqual(45, behavior.ModifyIncomingDamage(45));
        Assert.AreEqual(0, behavior.ModifyIncomingDamage(0));
        Assert.AreEqual(100, behavior.ModifyIncomingDamage(100));
    }

    #endregion

    #region Affects & Overwhelms Default

    [Test]
    public void Affects_Default_DoesNothingButCallBase()
    {
        var behavior = Create<Damage>();
        var target = new StatSheet();

        // Should not throw and should call base (no crash)
        Assert.DoesNotThrow(() => behavior.Affects(target));
    }

    [Test]
    public void Overwhelms_Default_CallsAffects()
    {
        var behavior = Create<Damage>();
        var target = new StatSheet();

        Assert.DoesNotThrow(() => behavior.Overwhelms(target));
    }

    #endregion
}