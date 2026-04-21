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
            this.maxValue = initial;
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
                    return maxValue;
                }
            } set => maxValue = value;
        }

        public override int Current
        {
            get
            {
                int raw = remaining + buff - debuff;
                return binding == ResourceBinding.Bound
                    ? Mathf.Clamp(raw, 0, baseValue)
                    : raw;                    // Unbound = no clamping
            }

            set
            {
                if (binding == ResourceBinding.Bound)
                {
                    remaining = Mathf.Clamp(value, 0, baseValue);
                }
                else
                {
                    remaining = value;        // Unbound can be negative or very high
                }
            }
        }

        public int Remaining
        {
            get => remaining;

            set => remaining = value;
        }
    }
}