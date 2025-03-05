using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.AbilitySystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RPG.StatSystem { 
    [System.Serializable]
    public class StatSheet
    {
        [SerializeField] private Stat health;
        [SerializeField] private Stat momentum;
        [SerializeField] private Stat motive;
        [SerializeField] private Stat means;
        [SerializeField] private Stat skill;

        private List<ABehavior> continuingEffects;

        /*
        private int healthInt;
        private int momentumInt;
        private int motiveInt;
        private int meansInt;
        private int skillInt;*/

        public StatSheet()
        {
            continuingEffects = new List<ABehavior>();

            health = new Stat(10);
            momentum = new Stat(10);
            motive = new Stat(10);
            means = new Stat(10);
            skill = new Stat(1);

            health.SetName("HEALTH");
            momentum.SetName("MOMENTUM");
            motive.SetName("MOTIVE");
            means.SetName("MEANS");
            skill.SetName("SKILL");

            health.SetStaticValue(false);
            momentum.SetStaticValue(false);
            motive.SetStaticValue(true);
            means.SetStaticValue(true);
            skill.SetStaticValue(true);
        }
        /*
        public void AbilityHit(Ability ability)
        {
            //Run checks to see if the ability hits.
            //If it does
            AbilityHit(ability.GetBehaviors());
        }*/


        public string characterName;

        public string GetName()
        {
            return characterName; //I plan to cosmetic stuff into its own class that links to this class. To get the UI set up I'm using this temp class
        }

        public void AbilityHit(List<ABehavior> input)
        {
            //Debug.Log("AbilityHit was called");
            

            foreach (ABehavior a in input)
            {
                //Debug.Log("Foreach loop reached");
                a.OnHit(this);
                //Check if a is supposed to stay on for extra rounds, turns, etc and if it is add them to the list of continuing effects
                if (!a.Continues())
                    continuingEffects.Add(a);

            }
        }

        public void TakesDamage(int amount)
        {
            //Debug.Log($"Takes damage. Health should now be {GetHealthBase() - amount}");
            SetHealthBase(GetHealthBase() - amount);
            //Debug.Log($"{GetHealthCurrent()}");
        }

        public void Heal(int amount)
        {
            TakesDamage(amount * -1);
        }

        public void Buff(string statName, int amount)
        {
            Stat stat = GetStatByName(statName);
            stat.ApplyBuff(amount);
        }

        public void Debuff(string statName, int amount)
        {
            Stat stat = GetStatByName(statName);
            stat.ApplyDebuff(amount);
        }


        public Stat[] GetStats()
        {
            //return this.FindObjectsOfType<Stat.class>();
            Stat[] myStats = { health, momentum, motive, means, skill };
            return myStats;
        }

        public List<string> GetStatNames()
        {
            List<string> allStatNames = new List<string>();
            foreach (Stat stat in GetStats())
            {
                allStatNames.Add(stat.GetName());
            }

            return allStatNames;
        }



        public Stat GetStatByName(string statName)
        {
            foreach(Stat stat in GetStats())
            {
                if(statName == stat.GetName())
                {
                    return stat;
                }
            }

            return null;
            //throw new ArgumentNotFoundException("Stat name not found in GetStatByName");
        }

        public void NewRound()
        {
            if(GetHealthCurrent() > 0)
            {
                int amount = GetMomentumCurrent() + GetMotiveCurrent();
                SetMomentumCurrent(amount);
            }
        }



        public Stat GetHealth()
        {
            return health;
        }
        public int GetHealthMax()
        {
            return health.GetMax();
        }
        public int GetHealthMin()
        {
            return health.GetMin();
        }
        public int GetHealthCurrent()
        {
            return health.GetCurrent();
        }
        public int GetHealthBase()
        {
            return health.GetBaseStat();
        }
        public void SetHealth(Stat health)
        {
            this.health = health;
        }
        public void SetHealthMax(int max)
        {
            this.health.SetMax(max);
        }
        public void SetHealthBase(int current)
        {
            this.health.SetBase(current);
        }


        public Stat GetSkill()
        {
            return skill;
        }
        public int GetSkillMax()
        {
            return skill.GetMax();
        }
        public int GetSkillMin()
        {
            return skill.GetMin();
        }
        public int GetSkillCurrent()
        {
            return skill.GetCurrent();
        }
        public void SetSkill(Stat a)
        {
            this.skill = a;
        }
        public void SetSkillMax(int max)
        {
            this.skill.SetMax(max);
        }
        public void SetSkillCurrent(int current)
        {
            this.skill.SetBase(current);
        }


        public Stat GetMomentum()
        {
            return momentum;
        }
        public int GetMomentumMax()
        {
            return momentum.GetMax();
        }
        public int GetMomentumMin()
        {
            return momentum.GetMin();
        }
        public int GetMomentumCurrent()
        {
            return momentum.GetCurrent();
        }
        public void SetMomentum(Stat momentum)
        {
            this.momentum = momentum;
        }
        public void SetMomentumMax(int max)
        {
            momentum.SetMax(max);
        }
        public void SetMomentumCurrent(int current)
        {
            momentum.SetBase(current);
        }




        public Stat GetMeans()
        {
            return means;
        }
        public int GetMeansMax()
        {
            return means.GetMax();
        }
        public int GetMeansMin()
        {
            return means.GetMin();
        }
        public int GetMeansCurrent()
        {
            return means.GetCurrent();
        }
        public void SetMeans(Stat means)
        {
            this.means = means;
        }
        public void SetMeansMax(int max)
        {
            means.SetMax(max);
        }
        public void SetMeansCurrent(int current)
        {
            means.SetBase(current);
        }




        public Stat GetMotive()
        {
            return motive;
        }
        public int GetMotiveMax()
        {
            return motive.GetMax();
        }
        public int GetMotiveMin()
        {
            return motive.GetMin();
        }
        public int GetMotiveCurrent()
        {
            return motive.GetCurrent();
        }

        public void SetMotive(Stat motive)
        {
            this.motive = motive;
        }
        public void SetMotiveMax(int max)
        {
            motive.SetMax(max);
        }
        public void SetMotiveCurrent(int current)
        {
            motive.SetBase(current);
        }






    }
}