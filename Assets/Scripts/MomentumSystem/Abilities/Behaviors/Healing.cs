using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    public class Healing : ABehavior
    {
        public override void Apply(StatSheet target)
        {
            target.Heal(amount);
        }
    }
}