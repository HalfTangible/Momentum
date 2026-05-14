using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
//using UnityEditor;

namespace RPG.AbilitySystem
{
    //[DisplayName("Healing - Health Recovery")]
    [System.Serializable]
    public class Healing : ABehavior
    {
        public override void Affects(StatSheet target)
        {

            Debug.Log("Heal triggered.");
            target.Heals((int)GetStat<int>("AMOUNT"));
            

            base.Affects(target);
        }

        public override bool EachTurn(StatSheet target)
        {
            if (getTurns() > 0)
            {
                SpendTurn();
                Affects(target);
            }
            return Continues();
        }

        public override bool EachRound(StatSheet target)
        {
            if (getRounds() > 0)
            {
                SpendRound();
                Affects(target);
            }
            return Continues();
        }

        public override void Overwhelms(StatSheet target)
        {
            Affects(target);
        }

        public override void Initialize(int amount)
        {
            Initialize(amount, true);
        }

        public void Initialize(int amount, bool onHit)
        {
           
            Initialize(amount, onHit, 0, 0);

        }

        public void Initialize(int amount, bool onHit, int rounds, int turns)
        {

            base.InitializeStats(amount, onHit, rounds, turns);

        }


    }
}