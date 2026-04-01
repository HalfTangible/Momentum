using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace RPG.StatSystem
{

    [System.Serializable]
    public abstract class Stat
    {
        [SerializeField] protected string name;
        [SerializeField] protected int baseValue;
        [SerializeField] protected int buff;
        [SerializeField] protected int debuff;

        // Public property for current value, with clamping logic
        public abstract int Current
        {
            get; set;
        }

        public abstract int Min
        {
            get; set;
        }

        public virtual string Name
        {
            get => name;
            set => name = value;
        }

        public virtual int Base
        {
            get => baseValue;
            set => baseValue = value;
        }

        public virtual int Buff
        {
            get => buff;
            set => buff = value;
        }

        public virtual int Debuff
        {
            get => debuff;
            set => debuff = value;
        }

        public Stat(int initial)
        {
            baseValue = initial;
            buff = 0;
            debuff = 0;
        }

        //public void ApplyBuff(int amount) => buff += amount;
        public virtual void ApplyBuff(int amount)
        {
            Debug.Log($"[Stat.ApplyBuff] {Name}: buff += {amount} (old buff: {buff}, new buff: {buff + amount})");
            buff += amount;
        }
        public virtual void ApplyDebuff(int amount) => debuff += amount;

        //public virtual int GetMin() => type == StatType.Character ? 1 : 0;

        public virtual void LevelUp(int amount)
        {
            // Add level-up logic here once we decide how level ups will work.
        }
    }
}