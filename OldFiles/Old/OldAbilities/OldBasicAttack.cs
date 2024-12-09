using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeededEnums;

public class BasicAttack : Ability
{
    //Does damage. That's it.
    Damage damageBehavior;
    public bool overwhelm { get; set; }
    public List<AbilityBehavior> abilityBehaviors { get; set; }
    
    public BasicAttack()
    {
        abilityBehaviors = new List<AbilityBehavior>();
        damageBehavior = new Damage();
        //AssignBehaviors(basicAttack); //Consider doing regular inheritance instead of an interface so you can just call the superclass?
    }

    public void AssignBehavior(AbilityBehavior a)
    {
        abilityBehaviors.Add(a);
    }

    public void AssignBehaviors(List<AbilityBehavior> a)
    {
        abilityBehaviors.AddRange(a);
    }

    public void OnOverwhelm(StatSheet user, StatSheet target)
    {
        //For if the ability adds a new behavior on Overwhelm.
        //Basic attack does not but it's in the AbilityBehavior interface so it's gotta be here

        damageBehavior.amount *= 2;

    }


    public bool UseAbility(StatSheet user, StatSheet target)
    {
        damageBehavior.amount = 3;
        damageBehavior.abilityTiming = AbilityTiming.OnHit;

        //Check if the ability Overwhelms, and if it does, add the Overwhelm stuff. In the basic attack's case we'll double damage.
        //Since we're doing this in a lot of places we should have a BattleUtility class for this kind of check.
        //Note: We might want to bring in potential overwhelm effects from the user and target in the future. So pass them.

        if (BattleUtility.CheckOverwhelm(user, target))
            OnOverwhelm(user, target);

        AssignBehavior(damageBehavior);

        //Either way, assign the behaviors to the enemy's character sheet.
        //Once that's done, the object created with this class should be deleted. Like a projectile.

        target.AbilityHit(abilityBehaviors);

        return true;
    }
}
