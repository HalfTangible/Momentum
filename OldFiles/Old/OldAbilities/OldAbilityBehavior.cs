using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeededEnums;

public interface AbilityBehavior
{
    //public StatSheet user { get; set; }
    //public StatSheet target { get; set; }
    //These behaviors will be added to ability prefabs
    public bool Perform(StatSheet target);
    public bool OnHit(StatSheet target); //What the ability does when it first hits
    public bool OnEnd(StatSheet target); //What the ability does when it ends its duration. For buffs and the like.
    public bool EachTurn(StatSheet target); //Check if there's any rounds remaining on the behavior. True if there's more, false if there's none.
    public bool EachRound(StatSheet target); //Check if there's any turns remaining on the behavior. True if there's more, false if there's none.
    public bool EachAction(StatSheet target);
    public bool Finished(); //Checks each of its duration types.
    
    public int TurnDuration { get; set; }
    public int RoundDuration { get; set; }
    public AbilityTiming abilityTiming { get; set; }
    //public AbilityTiming abilityTiming;
}


