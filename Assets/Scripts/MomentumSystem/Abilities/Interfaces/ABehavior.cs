using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    public abstract class ABehavior : ScriptableObject
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

        public T GetStat<T>(string key)
        {
            object value = FindValueByKey(key);
            
            UnityEngine.Debug.Log($"Key: {key}, Value: {value}, Value Type: {value?.GetType()}");
            UnityEngine.Debug.Log($"{stats.Count} in list.");
            List<string> temp1 = new List<string>();
            
                temp1.AddRange(GetAllKeys());

            foreach (string key in temp1)
                UnityEngine.Debug.Log($"Getting all keys: {key}, ");

            if (value is T typedValue)
            {
                return typedValue;
            }
            else if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"The value associated with key '{key}' is not of type {typeof(T).Name}.");
            }


        }

        public void SetStat(string key, object value)
        {
            key = key.ToUpperInvariant();

            FindStatByKey(key).value = value;

            //throw new KeyNotFoundException($"The key '{key}' is not valid for this behavior.");
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
            UnityEngine.Debug.Log($"Finding value by key: {key}");
            
            return FindStatByKey(key).value;
        }
        
        public BehaviorStat FindStatByKey(string key)
        {
            UnityEngine.Debug.Log($"Finding stat by key: {key}");
            foreach (BehaviorStat stat in stats)
            {
                if (stat.key == key)
                {
                    UnityEngine.Debug.Log($"Stat found!");
                    return stat;
                }
            }
           
                throw new KeyNotFoundException($"The key '{key}' was not found in the stats pseudo-dictionary.");


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

        public List<string> GetAllKeys()
        {
            UnityEngine.Debug.Log("Getting all keys");
            List<string> allKeys = new List<string>();

            foreach (BehaviorStat stat in stats)
            {
                UnityEngine.Debug.Log($"Getting key: {stat.key}");
                allKeys.Add(stat.key);
            }

            return allKeys;
        }

        public void AddStat(string key, object value)
        {
            AddStat(new BehaviorStat(key, value));
        }

        public void AddStat(BehaviorStat stat)
        {
            stats.Add(stat);
        }

        public virtual void Initialize(int amount)
        {
            InitializeStats(amount, true, 0, 0);
        }

        protected virtual void InitializeStats(int amount, bool onHit, int roundsRemaining, int turnsRemaining)
        {
            stats.Clear();
            stats.Add(new BehaviorStat("AMOUNT", amount));
            stats.Add(new BehaviorStat("ONHIT", onHit));
            stats.Add(new BehaviorStat("ROUNDS", roundsRemaining));
            stats.Add(new BehaviorStat("TURNS", turnsRemaining));

            //if you wanna use InitializeStats in a child class call it with base
            //Like this: base.InitializeStats(amount,onHit,rounds,turns);
        }

    }
    [System.Serializable]
    public class BehaviorStat
    {
        //Because apparently dictionaries aren't serializable. Which is stupid.
        //Like, I get it, but it's stupid to have to work through.
        [SerializeField]
        public string key;
        [SerializeField]
        public object value;

        public BehaviorStat(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

    }
}