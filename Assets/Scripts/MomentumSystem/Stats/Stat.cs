using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace RPG.StatSystem
{

    public enum StatType
    {
        Character, // e.g., skill, motive, means; min 1, no max
        Resource,  // e.g., health, mana; min 0, has max
        Unbounded  // e.g., Momentum; no max, can go negative
    }

    [System.Serializable]
    public class Stat
    {
        [SerializeField] private string name;
        [SerializeField] private StatType type;
        [SerializeField] private int max;
        [SerializeField] private int baseStat;
        [SerializeField] private int buff;
        [SerializeField] private int debuff;

        public string getValues()
        {
            return $"Name: {name}, type: {type}, Max: {max}, baseStat: {baseStat}, buff: {buff}, debuff: {debuff}";
        }

        // Public property for current value, with clamping logic
        public int Current
        {
            get
            {
                int current = baseStat + buff - debuff;
                switch (type)
                {
                    case StatType.Character:
                        return Mathf.Max(current, GetMin()); // Min 1, no max
                    case StatType.Resource:
                        return Mathf.Clamp(current, GetMin(), max); // Min 0, max defined
                    case StatType.Unbounded:
                        return current; // No limits
                    default:
                        Debug.LogWarning($"Unknown StatType for {name}: {type}");
                        return current;
                }
            }
            set => baseStat = value; // Updates baseStat, current applies clamping
        }

        // Public fields for direct access, still serialized for Inspector
        public int Max
        {
            get => max;
            set => max = type == StatType.Resource ? value : 0;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Base
        {
            get => baseStat;
            set => baseStat = value;
        }

        public int Buff
        {
            get => buff;
            set => buff = value;
        }

        public int Debuff
        {
            get => debuff;
            set => debuff = value;
        }

        public Stat(int initial, StatType type)
        {
            this.type = type;
            max = type == StatType.Resource ? initial : 0;
            baseStat = initial;
            buff = 0;
            debuff = 0;
        }

        //public void ApplyBuff(int amount) => buff += amount;
        public void ApplyBuff(int amount)
        {
            Debug.Log($"[Stat.ApplyBuff] {Name}: buff += {amount} (old buff: {buff}, new buff: {buff + amount})");
            buff += amount;
        }
        public void ApplyDebuff(int amount) => debuff += amount;

        public int GetMin() => type == StatType.Character ? 1 : 0;

        public void LevelUp(int amount)
        {
            // Add level-up logic here once we decide how level ups will work.
        }
    }
}