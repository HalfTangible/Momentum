using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System;
using Debug = UnityEngine.Debug;
//using UnityEditor;

namespace RPG.AbilitySystem
{
    //[DisplayName("Damage - Reduce health")]
    [System.Serializable]
    public class Damage : ABehavior
    {
        //int amount;
        //int roundsRemaining;
        //int turnsRemaining;
        //bool onHit;
        //List<string> stats = { "AMOUNT", "ROUNDS", "TURNS", "ONHIT" }
        //private Dictionary<string, object> stats; 
        /*public void Apply(StatSheet user, StatSheet target)
        {
            //Attaches this behavior to the target. It then triggers based on
            //target.AbilityHit(this);

            //Actually... apply would need to be used by the ability, wouldn't it?
        }*/

        public override bool EachTurn(StatSheet target)
        {
            
            Debug.Log("[Damage.EachTurn()] turnsRemaining 1:" + getTurns());
            if(getTurns() > 0)
            {

                Debug.Log("[Damage.EachTurn()] turnsRemaining 2: " + getTurns());
                Affects(target);
                Debug.Log("[Damage.EachTurn()] turnsRemaining 3: " + getTurns());
            }

            Debug.Log("[Damage.EachTurn()] turnsRemaining 4: " + getTurns());

            return base.EachTurn(target);
        }
        

        public override bool EachRound(StatSheet target)
        {
            Debug.Log("[Damage.EachRound()] roundsRemaining 1: " + getRounds());
            if (roundsRemaining > 0)
            {

                Debug.Log("[Damage.EachRound()] roundsRemaining 2: " + getRounds());
                Affects(target);
                Debug.Log("[Damage.EachRound()] After Affects, roundsRemaining 3: " + getRounds());
            }

            Debug.Log("[Damage.EachRound()] roundsRemaining 4: " + getRounds());

            return base.EachRound(target);
        }


        public override void Finished(StatSheet target) 
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            base.Finished(target);
        }
        
        public override void Affects(StatSheet target)
        {
            int finalDamage = target.ApplyDefenses((int)GetStat<int>("AMOUNT"));

            Debug.Log("[Damage.Affects()] initial damage: " + finalDamage);
            if (finalDamage > 0)
                target.TakesDamage(finalDamage);

            Debug.Log("[Damage.Affects()] final Damage: " + finalDamage);

            base.Affects(target);

            //With the OnHit done, we check to see if the effect continues.
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
            //By default, damage will happen once on hit.
            Initialize(amount, onHit, 0, 0);


        }

        public void Initialize(int amount, bool onHit, int rounds, int turns)
        {

            base.InitializeStats(amount, onHit, rounds, turns);

        }

        private string Description()
        {
            string desc = "Damaging ability. \n";
            int amount = (int) GetStat<int>("AMOUNT");
            bool onHit = (bool) GetStat<bool>("ONHIT");
            int rounds = (int) GetStat<int>("ROUNDS");
            int turns = (int) GetStat<int>("TURNS");

            if (onHit)
                desc += $"*On hit, do {amount} damage.\n";
            if (rounds == 1)
                desc += $"*Do {amount} damage at the start of the next round.";
            if (turns == 1)
                desc += $"Do {amount} damage at the start of the target's next turn.";
            if (rounds > 1)
                desc += $"*Do {amount} damage at the start of each round for {rounds} rounds";
            if (turns > 1)
                desc += $"Do {amount} damage each turn at the start of each of the target's turns for {turns} turns.";


            return desc;
        }

    }
}