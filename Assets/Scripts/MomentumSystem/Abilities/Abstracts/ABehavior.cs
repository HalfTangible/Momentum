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
        protected int amount;
        [SerializeField]
        protected int roundsRemaining;
        [SerializeField]
        protected int turnsRemaining;
        [SerializeField]
        protected bool onHit;

        [SerializeField]
        protected List<string> allKeys;
        //bool Continues();
        //bool Finished();
        //void OnHit(StatSheet target);
        //private Dictionary<string, object> stats;
        //public List<string> GetKeys();

        //ABehavior.cs

        public ABehavior()
        {
            allKeys = new List<string>();
            allKeys.AddRange(new[] { "AMOUNT", "ONHIT", "ROUNDS", "TURNS" });
            allKeys.Sort();
        }

        public List<string> GetAllKeys()
        {
            return allKeys;
        }

        public virtual bool EachRound(StatSheet target)
        {
            return Continues();
        }

        public virtual bool EachTurn(StatSheet target)
        {
            return Continues();
        }


        public virtual void OnHit(StatSheet target)
        {
            //This exists to be overwritten in other classes. By having it here and overridden, Behaviors can all be called this way but will have different effects based on
            //its actual effect.
        }

        
        public virtual void Apply(StatSheet target)
        {
            //This exists to be overwritten in other classes. By having it here and overridden, Behaviors can all be called this way but will have different effects based on
            //its actual effect.
        }

        public bool Continues()
        {
            //Called at the end of apply. The code checks to see if the ability is done.
            if (roundsRemaining > 0 || turnsRemaining > 0)
                return true;
            else return false;
        }

        public virtual void Finished(StatSheet target)
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            //return false;
            //This used to be a bool but I don't recall a reason for it. Continues needs to check, Finished won't be called until after it does anyway.
            //This behavior then deletes itself.
        }

        public virtual object GetStat<T>(string key)
        {
            //When calling this from a child class (base.GetStat(key)) make sure to do data fields unique to the child class first.

            switch (key)
            {
                case "AMOUNT":
                    if (amount is T tAmount)
                        return tAmount;
                    break;

                case "ONHIT":
                    if (onHit is T tOnHit)
                        return tOnHit;
                    break;

                case "ROUNDS":
                    if (roundsRemaining is T tRounds)
                        return tRounds;
                    break;

                case "TURNS":
                    if (turnsRemaining is T tTurns)
                        return tTurns;
                    break;

                default:
                    throw new KeyNotFoundException($"The key '{key}' is not recognized.");
            }

            throw new InvalidCastException($"The value associated with key '{key}' cannot be cast to type {typeof(T).Name}.");
        }

        public virtual string GetListKey(string key) //Used to retrieve the key to the variable used to store the value of a given list
        {
            switch (key)
            {


                default:
                    throw new KeyNotFoundException($"The key '{key}' is not recognized.");
            }


        }

        public virtual void SetStat(string key, object value)
        {
            // Convert the object to a string representation and call the string-based SetStat
            if (value != null)
            {
                SetStat(key, value.ToString());
            }
            else
            {
                throw new ArgumentException($"Value for key '{key}' cannot be null.");
            }
        }

        public virtual int GetListIndex(string key)
        {

            return 0;
        }


        public virtual void SetStat(string key, string value)
        {
            //UnityEngine.Debug.Log($"Set stats Method. Key = {key}, value = {value}");
            switch (key)
            {
                case "AMOUNT":
                    amount = ParseInput<int>(value); // Now correctly parses the string to an int.
                    break;

                case "ONHIT":
                    onHit = ParseInput<bool>(value); // Now correctly parses the string to a bool.
                    break;

                case "ROUNDS":
                    roundsRemaining = ParseInput<int>(value); // Parses to an int.
                    break;

                case "TURNS":
                    turnsRemaining = ParseInput<int>(value); // Parses to an int.
                    break;

                default:
                    throw new ArgumentException($"Unknown key: {key}");
            }
        }

        public virtual void SetStat(string key, List<string> values)
        {
            switch (key)
            {


                default:
                    throw new ArgumentException($"Unknown key: {key}");
            }


        }

        public virtual void Initialize(int amount)
        {
            InitializeStats(amount, true, 0, 0);
        }

        protected virtual void InitializeStats(int amount, bool onHit, int roundsRemaining, int turnsRemaining)
        {

            SetStat("AMOUNT", amount);
            SetStat("ONHIT", onHit);
            SetStat("ROUNDS", roundsRemaining);
            SetStat("TURNS", turnsRemaining);

            //if you wanna use InitializeStats in a child class call it with base
            //Like this: base.InitializeStats(amount,onHit,rounds,turns);
        }


        protected virtual T ParseInput<T>(string input)
        {
            //UnityEngine.Debug.Log($"ParseInput Method. Key = {input}, typeOf = {typeof(T)}");

            if (typeof(T) == typeof(string))
            {
                return (T)(object)input;
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(input, out int intValue))
                    return (T)(object)intValue;
                throw new ArgumentException("Expected an integer value.");
            }
            else if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(input, out bool boolValue))
                    return (T)(object)boolValue;
                throw new ArgumentException("Expected a boolean value.");
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported by the ParseInput method, dude.");
            }
            /*
            switch (typeof(inputType))
            {
                case int:
                    if(int.TryParse(input, out int intValue))
                        return intValue;
                    throw new ArgumentException("Expected an integer value");
                case bool:
                    if (bool.TryParse(input, out bool boolValue))
                        return boolValue;
                    throw new ArgumentException("Expected a bool value");
                default:
                    return input;
            }*/
        }


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
        /*
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

        protected virtual void InitializeStats(int amount, bool onHit, int roundsRemaining, int turnsRemaining)
        {
            stats.Clear();
            stats.Add(new BehaviorStat("AMOUNT", amount));
            stats.Add(new BehaviorStat("ONHIT", onHit));
            stats.Add(new BehaviorStat("ROUNDS", roundsRemaining));
            stats.Add(new BehaviorStat("TURNS", turnsRemaining));

            //if you wanna use InitializeStats in a child class call it with base
            //Like this: base.InitializeStats(amount,onHit,rounds,turns);
        }*/

    
    /*
    [System.Serializable]
    public class BehaviorStat
    {
        //Because apparently dictionaries aren't serializable. Which is stupid.
        //Like, I get it, but it's stupid to have to work through.
        //It doesn't serialize objects either. So I guess we're doing simple data types because there's no way doing a couple hundred JSON actions at the start of a game is worth it.
        //Yeah it'd be easier to code at this point but we're trying to be efficient with the game's memory usage right now
        
        [SerializeField]
        public string key;
        [SerializeField]
        public object value;

        public BehaviorStat(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

    }*/

    
