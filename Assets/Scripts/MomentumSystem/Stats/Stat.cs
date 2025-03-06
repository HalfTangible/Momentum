using System.Collections;
using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace RPG.StatSystem {
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private string name;
        [SerializeField] private int max;
        [SerializeField] private bool isStaticValue = false;
        [SerializeField] private int baseStat;
        [SerializeField] private int buff;
        [SerializeField] private int debuff;

        public Stat(int initial)
        {
            max = initial;
            baseStat = initial;
            isStaticValue = false;
        }

        public void ApplyBuff(int buff)
        {
            this.buff += buff;
        }

        public void ApplyDebuff(int debuff)
        {
            this.debuff += debuff; //Gets subtracted elsewhere so needs to be positive here
        }

        public string GetName() => name;
        public void SetName(string name) => this.name = name;

        public int GetMax() => max;
        public void SetMax(int max) => this.max = max;


        public int GetMin() => isStaticValue ? 1 : 0;

        public bool GetStaticValue() => isStaticValue;

        public void SetStaticValue(bool value) => isStaticValue = value;

        public int GetBaseStat() => baseStat;

        public void SetCurrent(int newAmount)
        {
            SetBase(newAmount);
        }

        public int GetCurrent()
        {

            int current = this.baseStat + buff - debuff;
            //Hokay... so... if it's a static value, the minimum is 1 and it has no maximum
            //If it's not a static value, then it need a maximum and its minimum is 0
            int min = GetMin();

            //Momentum should have no max or min. It is NOT a static value, but its max is set to 0.

            if (!isStaticValue && max == 0) //If this is the case (just Momentum as of yet) return current. No notes.
                return current;
            else if (isStaticValue) //If it IS a static value (ie a stat) then it has a minimum of 1 and no max.
                return Mathf.Max(current, min);
            else //If it is NOT a static value (ie health or mana or the like) then it has both a max and minimum value
                return Mathf.Clamp(current, min, max);

            // Clamp the value: no max if static, otherwise limit to max
            // If max is 0, treat it as no max
  
/*            return isStaticValue
                ? Mathf.Max(current, min)
                : Mathf.Clamp(current, min, max);*/

                /*        if (current < min)
                            return min;

                        if (!isStaticValue && current > max)
                            return max;

                        return current;*/

        }

        public void SetBase(int newBase) => baseStat = newBase; //Probably only going to be used on a level-up anyway. REMEMBER TO RESET THE MAX FIRST!!

        public void LevelUp(int amount)
        {
        //REMEMBER TO RESET THE MAX IF IT'S NOT 0 ALREADY
        }
    }
}