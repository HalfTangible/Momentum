using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    public abstract class ABehavior
    {
        //void Apply(StatSheet user, StatSheet target);
        [SerializeField]
        protected List<BehaviorStat> stats;

        //bool Continues();
        //bool Finished();
        //void OnHit(StatSheet target);
        //private Dictionary<string, object> stats;
        //public List<string> GetKeys();

        //ABehavior.cs

        public ABehavior()
        {
            stats = new List<BehaviorStat>();
        }
        
        public virtual void OnHit(StatSheet target)
        {
            //Don't forget to implement the effects in the child classes when you override them.
            //With the OnHit done, we check to see if the effect continues.
            //If it does, we apply the ability to the target. They'll handle it from there.
            //Actually that function seems to have been implemented in StatSheet already... hrm.
            //Move it here, perhaps?
            //Nah, if we implement armor or something we'll want to check WhenHit() or something.
        }

        public bool Continues()
        {
            //Called at the end of apply. The code checks to see if the ability is done.
            if (GetStat<int>("ROUNDS") > 0 || GetStat<int>("TURNS") > 0)
                return true;
            else return false;
        }

        public bool Finished()
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            return false;
        }

        public virtual List<string> GetKeys()
        {
            List<string> keys = new List<string>();
            foreach (BehaviorStat stat in stats)
            {
                keys.Add(key);
            }
            return keys;
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

        public object FindValueByKey(string key)
        {
            FindStatByKey(key).value;
        }
        
        public BehaviorStat FindStatByKey(string key)
        {
            foreach (BehaviorStat stat in stats)
            {
                if (stat.key == key)
                    return stat;
            }
        }

        public void UpdateStatByKey(string key, object value)
        {
            foreach(BehaviorStat stat in stats)
            {
                if (stat.key == key)
                {
                    stat.value = value;
                    return;
                }
            }
        }
        
    }

    public class BehaviorStat
    {
        //Because apparently dictionaries aren't serializable. Which is stupid.
        //Like, I get it, but it's stupid to have to work through.

        public string key;
        public object value;

        public BehaviorStat(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

    }
}