using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System;

namespace RPG.AbilitySystem
{
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

        public bool EachTurn(StatSheet target)
        {
            int turnsRemaining = GetStat<int>("TURNS");

            if(turnsRemaining > 0)
            {
                target.TakesDamage(GetStat<int>("AMOUNT"));
                SetStat("TURNS", --turnsRemaining);
            }


            
            return Continues();
        }

        public bool EachRound(StatSheet target)
        {
            int roundsRemaining = GetStat<int>("ROUNDS");

            if (roundsRemaining > 0)
            {
                target.TakesDamage(GetStat<int>("AMOUNT"));
                SetStat("ROUNDS", --roundsRemaining);
            }
            
            return Continues();
        }



        public bool Finished() 
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            return false;
        }
        
        public override void OnHit(StatSheet target)
        {

            if (GetStat<bool>("ONHIT"))
                target.TakesDamage(GetStat<int>("AMOUNT"));

            base.OnHit(target);

            //With the OnHit done, we check to see if the effect continues.
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

            InitializeStats(amount, onHit, rounds, turns);

        }

        private string Description()
        {
            string desc = "Damaging ability. \n";
            int amount = GetStat<int>("AMOUNT");
            bool onHit = GetStat<bool>("ONHIT");
            int rounds = GetStat<int>("ROUNDS");
            int turns = GetStat<int>("TURNS");

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


        /*
        public List<string> GetStats()
        {
            return Stats;
        }

        public T GetThis<T>(string toGet.ToUppercase())
        {
            switch(toGet):
                case "AMOUNT":
                return amount;

                case "ONHIT":
                return onHit;

                case "ROUNDS":
                return roundsRemaining;

                case "TURNS":
                return turnsRemaining;


        }*/
    }
}