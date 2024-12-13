using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.AbilitySystem;

namespace RPG.StatSystem { 
    [System.Serializable]
    public class StatSheet
    {
        [SerializeField] private Stat health;
        [SerializeField] private Stat momentum;
        [SerializeField] private Stat motive;
        [SerializeField] private Stat means;
        [SerializeField] private Stat skill;

        private List<IBehavior> continuingEffects;

        /*
        private int healthInt;
        private int momentumInt;
        private int motiveInt;
        private int meansInt;
        private int skillInt;*/

        public StatSheet()
        {
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

        public void AbilityHit(List<IBehavior> input)
        {
            foreach (IBehavior a in input)
            {
                a.OnHit(this);
                //Check if a is supposed to stay on for extra rounds, turns, etc and if it is add them to the list of continuing effects
                if (!a.Finished())
                    continuingEffects.Add(a);

            }
        }

        public Stat[] GetStats()
        {
            //return this.FindObjectsOfType<Stat.class>();
            Stat[] myStats = { health, momentum, motive, means, skill };
            return myStats;
        }

        public void TakesDamage(int amount)
        {
        
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
            motive.SetBase(current);
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





        /*
        public void setHealth(Stat health)
        {
            this.health = health;
        }

        public void setMomentum(Stat momentum)
        {
            this.momentum = momentum;
        }

        public void setMotive(Stat motive)
        {
            this.motive = motive;
        }

        public void setMeans(Stat means)
        {
            this.means = means;
        }*/



    }
}