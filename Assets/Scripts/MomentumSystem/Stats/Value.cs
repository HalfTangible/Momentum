using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.StatSystem
{
    [System.Serializable]
    public class ValueStat : Stat
    {
        [SerializeField] protected int minValue = 1;

        public ValueStat(string name, int initial)
        {
            this.name = name;
            minValue = 1;
        }

        public override int Current
        {
            get => baseValue + buff - debuff;
            set => baseValue = value;           // usually just overwrite base (level up / perm change)
        }

        public override int Min
        {
            get => minValue;
            set => minValue = value;
        }

        public override int Max
        {
            get => maxValue;
            set => maxValue = value;
        }
    }
}