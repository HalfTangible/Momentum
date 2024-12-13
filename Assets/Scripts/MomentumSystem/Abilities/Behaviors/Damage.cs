using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System;

namespace RPG.AbilitySystem
{
    public class Damage : IBehavior
    {
        int amount;
        int roundsRemaining;
        int turnsRemaining;
        bool onHit;
        //List<string> stats = { "AMOUNT", "ROUNDS", "TURNS", "ONHIT" }
        private Dictionary<string, object> stats; 
        /*public void Apply(StatSheet user, StatSheet target)
        {
            //Attaches this behavior to the target. It then triggers based on
            //target.AbilityHit(this);

            //Actually... apply would need to be used by the ability, wouldn't it?
        }*/

        public bool EachTurn(StatSheet target)
        {
            if(turnsRemaining > 0)
            {
                target.TakesDamage(amount);
                turnsRemaining--;
            }
            
            return Continues();
        }

        public bool EachRound(StatSheet target)
        {
            if (roundsRemaining > 0)
            {
                target.TakesDamage(amount);
                roundsRemaining--;
            }
            
            return Continues();
        }

        public bool Continues()
        {
            //Called at the end of apply. The code checks to see if the ability is done.
            if (roundsRemaining > 0 || turnsRemaining > 0)
                return true; else return false;
        }

        public bool Finished() 
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            //Since this behavior is damage, we don't need to do that, just be done.
            return false;
        }
        
        public void OnHit(StatSheet target)
        {
            if (onHit)
                target.TakesDamage(amount);
            //With the OnHit done, we check to see if the effect continues.
        }

        public Damage(int amountEachTick) : this(amountEachTick, true)
        {

        }

        public Damage(int amountEachTick, bool onHit)
        {
            amount = amountEachTick;
            this.onHit = onHit;
        }

        public Damage(int amountEachTick, bool onHit, int rounds, int turns) : this(amountEachTick, onHit) { 
        
            roundsRemaining = rounds;
            turnsRemaining = turns;
                
        }


        public List<string> GetKeys()
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

                // Update the internal fields if necessary
                switch (key)
                {
                    case "AMOUNT":
                        amount = Convert.ToInt32(value);
                        break;
                    case "ONHIT":
                        onHit = Convert.ToBoolean(value);
                        break;
                    case "ROUNDS":
                        roundsRemaining = Convert.ToInt32(value);
                        break;
                    case "TURNS":
                        turnsRemaining = Convert.ToInt32(value);
                        break;
                }
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