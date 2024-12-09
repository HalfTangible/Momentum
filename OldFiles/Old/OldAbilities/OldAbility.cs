using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ability
{
    public bool overwhelm { get; set; }
    public List<AbilityBehavior> abilityBehaviors { get; set; }
    public void OnOverwhelm(StatSheet user, StatSheet target);
    public bool UseAbility(StatSheet user, StatSheet target);
    public void AssignBehaviors(List<AbilityBehavior> a);
    public void AssignBehavior(AbilityBehavior a);
}
