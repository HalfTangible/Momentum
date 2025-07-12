using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace RPG.StatSystem
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private string name;
        [SerializeField] private int max;
        [SerializeField] private bool isStaticValue = false;
        [SerializeField] private int baseStat;
        [SerializeField] private int buff;
        [SerializeField] private int debuff;

        // Public property for current value, with clamping logic
        public int Current
        {
            get
            {
                int current = baseStat + buff - debuff;
                int min = isStaticValue ? 1 : 0;
                if (!isStaticValue && max == 0) // Special case for momentum
                    return current;
                return isStaticValue ? Mathf.Max(current, min) : Mathf.Clamp(current, min, max);
            }
            set => baseStat = value; // Updates baseStat, current applies clamping
        }

        // Public fields for direct access, still serialized for Inspector
        public int Max
        {
            get => max;
            set => max = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public bool IsStaticValue
        {
            get => isStaticValue;
            set => isStaticValue = value;
        }

        public int BaseStat
        {
            get => baseStat;
            set => baseStat = value;
        }

        public Stat(int initial)
        {
            max = initial;
            baseStat = initial;
            buff = 0;
            debuff = 0;
            isStaticValue = false;
        }

        public void ApplyBuff(int amount) => buff += amount;
        public void ApplyDebuff(int amount) => debuff += amount;

        public int GetMin() => isStaticValue ? 1 : 0;

        public void LevelUp(int amount)
        {
            // Add level-up logic here if needed
        }
    }
}