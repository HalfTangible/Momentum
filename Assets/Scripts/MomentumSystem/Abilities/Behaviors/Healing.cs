using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    [DisplayName("Healing - Health Recovery")]
    [System.Serializable]
    public class Healing : ABehavior
    {
        public override void Affects(StatSheet target)
        {

            Debug.Log("Heal triggered.");
            target.Heals((int)GetStat<int>("AMOUNT"));
            

            base.Affects(target);
        }

        public override void Overwhelms(StatSheet target)
        {
            Affects(target);
        }


    }
}