using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.StatSystem
{

    public enum ResourceBinding
    {
        Bound,     // Normal resources (Health, Mana, Momentum, Stamina, etc.)
        Unbound    // Resources that can grow/shrink without limit (Rage, Combo Meter, Overdrive, etc.)
    }


    [System.Serializable]
    public class ResourceStat : Stat
    {
        [SerializeField] protected ResourceBinding binding; //If unbound, no max or min. If bound, there's a max and the min is 0.
        [SerializeField] protected int remaining;
        [SerializeField] protected int maxValue;
        [SerializeField] protected int minValue;

        public ResourceStat(string name, int initial, ResourceBinding binding)
            : base(initial)
        {
            this.name = name;
            this.baseValue = initial;
            this.binding = binding;
            this.remaining = initial;
            this.minValue = 0;
        }

        public override int Min
        {
            get {
                if (binding == ResourceBinding.Unbound) {
                    return -1;
                }
                else
                {
                    return minValue;
                }
            } set => minValue = value;
        }

        public int Max
        {
            get
            {
                if (binding == ResourceBinding.Unbound)
                {
                    return -1;
                }
                else
                {
                    return baseValue;
                }
            } set => maxValue = value;
        }

        public override int Current
        {
            get => (remaining + buff - debuff);
            
            set => remaining = value;
        }

        public int Remaining
        {
            get => remaining;

            set => remaining = value;
        }
    }
}