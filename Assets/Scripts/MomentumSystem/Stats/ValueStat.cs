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
            : base(initial)
        {
            this.name = name;
            minValue = 1;
            baseValue = initial;
        }

        public override int Current
        {
            get => Mathf.Max(minValue, baseValue + buff - debuff);
            set => baseValue = value;           // usually just overwrite base (level up / perm change)
        }

        public override int Min
        {
            get => minValue;
            set => minValue = value;
        }
    }
}