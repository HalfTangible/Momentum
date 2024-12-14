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

        public Damage(int amount) : this(amount, true)
        {
            
        }

        public Damage(int amount, bool onHit) : this(amount, onHit, 0, 0)
        {
            //By default, damage will happen once on hit.
            
            
        }

        public Damage(int amount, bool onHit, int rounds, int turns)
        {

            InitializeStats(amount, onHit, rounds, turns);

        }

        private void InitializeStats(int amount, bool onHit, int roundsRemaining, int turnsRemaining)
        {
            stats = new Dictionary<string, object>()
            {
                { "AMOUNT", amount },
                { "ONHIT", onHit },
                { "ROUNDS", roundsRemaining },
                { "TURNS", turnsRemaining }
            };
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



        public override List<string> GetKeys()
        {
            return new List<string>(stats.Keys);
        }
        
        public T GetStat<T>(string key)
        {
            if (stats.TryGetValue(key.ToUpperInvariant(), out object value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    throw new InvalidCastException($"The value associated with key '{key}' is not of type {typeof(T).Name}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"The key '{key}' was not found in the stats dictionary.");
            }
        }
        public void SetStat(string key, object value)
        {
            key = key.ToUpperInvariant();

            if (stats.ContainsKey(key))
            {
                stats[key] = value;
            }
            else
            {
                throw new KeyNotFoundException($"The key '{key}' is not valid for this behavior.");
            }
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