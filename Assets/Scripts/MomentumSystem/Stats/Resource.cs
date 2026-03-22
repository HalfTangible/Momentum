using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.StatSystem
{
    [System.Serializable]
    public class ResourceStat : Stat
    {
        [SerializeField] protected bool unbound; //If unbound, no max or min. If bound, there's a max and the min is 0.

        public ResourceStat(string name, int initial, bool unbound)
        {
            this.name = name;
            this.baseStat = initial;
            this.unbound = unbound;
        }

        public override int Min
        {
            get {
                if (unbound) {
                    return null;
                }
                else
                {
                    return value;
                }
            } set => minValue = value;
        }

        public override int Max
        {
            get
            {
                if (!unbound)
                {
                    return null;
                }
                else
                {
                    return value;
                }
            } set => maxValue = value;
        }
    }
}