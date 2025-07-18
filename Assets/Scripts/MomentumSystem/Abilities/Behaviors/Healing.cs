using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    public class Healing : ABehavior
    {
        public override void OnHit(StatSheet target)
        {
            //Debug.Log($"Damage on hit reached. Bool: {(bool)GetStat<bool>("ONHIT")}");

            if ((bool)GetStat<bool>("ONHIT"))
            {
                //Debug.Log($"It's onhit! Do damage: {(int)GetStat<int>("AMOUNT")}");
                target.Heals((int)GetStat<int>("AMOUNT"));
            }

            base.OnHit(target);

            //With the OnHit done, we check to see if the effect continues.
        }


    }
}