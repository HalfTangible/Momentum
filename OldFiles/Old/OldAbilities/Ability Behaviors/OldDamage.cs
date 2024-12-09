using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeededEnums;
using System;

[System.Serializable]
public class Damage : MonoBehaviour, AbilityBehavior
{
    
    public int amount = 0; //amount of damage done
    //public AbilityTiming abilityTiming;
    public int TurnDuration { get; set; }
    public int RoundDuration { get; set; }
    public AbilityTiming abilityTiming { get; set; }

    public bool Perform(StatSheet target)
    {
        return Perform(target.getHealth());
    }

    public bool Perform(Stat Health)
    {
        try
        {
            Health.setCurrent(Health.getCurrent() - amount);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool OnHit(StatSheet target) { return Perform(target); } //What the ability does when it first hits
    public bool OnEnd(StatSheet target) { return Perform(target); } //What the ability does when it ends its duration
    public bool EachTurn(StatSheet target) { return Perform(target); }
    public bool EachRound(StatSheet target) { return Perform(target); }
    public bool EachAction(StatSheet target) { return Perform(target); }
    public bool Finished()
    {
        return !(TurnDuration > 0 || RoundDuration > 0);
    }


}