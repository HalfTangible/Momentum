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
        protected bool onUser;
        [SerializeField]
        protected bool beforeHit;
        [SerializeField]
        protected bool afterHit;


        //[SerializeField]
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
            allKeys.AddRange(new[] { "AMOUNT", "ONHIT", "ROUNDS", "TURNS", "ONUSER", "BEFOREHIT", "AFTERHIT" });
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


        public virtual void Affects(StatSheet target)
        {
            //This exists to be overwritten in other classes. By having it here and overridden, Behaviors can all be called this way but will have different effects based on
            //its actual effect.
        }

        public virtual void Overwhelms(StatSheet target)
        {
            //This exists to be overwritten in other classes. By having it here and overridden, Behaviors can all be called this way but will have different effects based on
            //its actual effect.
        }
        /*
        public virtual void Apply(StatSheet target)
        {
            //This exists to be overwritten in other classes. By having it here and overridden, Behaviors can all be called this way but will have different effects based on
            //its actual effect.
        }*/

        public bool Continues()
        {
            //Called at the end of apply. The code checks to see if the ability is done.
            if (roundsRemaining > 0 || turnsRemaining > 0)
                return true;
            else return false;
        }

        public virtual void Finished(StatSheet target)
        {
            Debug.Log("ABehavior.Finished() called.");
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            //return false;
            //This used to be a bool but I don't recall a reason for it. Continues needs to check, Finished won't be called until after it does anyway.
            //This behavior then deletes itself.
        }

        public virtual object GetStat<T>(string key)
        {

            key = key.Trim().ToUpper();
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

                case "ONUSER":
                    if (onUser is T tOnUser)
                        return tOnUser;
                    break;

                case "BEFOREHIT":
                    if (beforeHit is T tBeforeHit)
                        return tBeforeHit;
                    break;

                case "AFTERHIT":
                    if (afterHit is T tAfterHit)
                        return tAfterHit;
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

            key = key.Trim().ToUpper();

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

                case "ONUSER":
                    onUser = ParseInput<bool>(value);
                    break;

                case "BEFOREHIT":
                    beforeHit = ParseInput<bool>(value);
                    break;

                case "AFTERHIT":
                    afterHit = ParseInput<bool>(value);
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
            SetStat("ONUSER", false);
            SetStat("BEFOREHIT", false);
            SetStat("AFTERHIT", false);

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

        }


    }
}

    
