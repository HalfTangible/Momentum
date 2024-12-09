using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using RPG.AbilitySystem;


public class StatSheet : MonoBehaviour
{
    private Stat health;
    private Stat momentum;
    private Stat motive;
    private Stat means;
    private Stat skill;

    private List<IBehavior> continuingEffects;

    public int healthInt;
    public int momentumInt;
    public int motiveInt;
    public int meansInt;
    public int skillInt;

    public StatSheet()
    {
        health = new Stat(healthInt);
        momentum = new Stat(momentumInt);
        motive = new Stat(motiveInt);
        means = new Stat(meansInt);
        skill = new Stat(skillInt);

        health.setName("HP");
        momentum.setName("MOMENTUM");
        motive.setName("MOTIVE");
        means.setName("MEANS");
        skill.setName("SKILL");

        health.setStaticValue(false);
        momentum.setStaticValue(false);
        motive.setStaticValue(true);
        means.setStaticValue(true);
        skill.setStaticValue(true);
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

    public Stat[] getStats()
    {
        //return this.FindObjectsOfType<Stat.class>();
        Stat[] myStats = { health, momentum, motive, means, skill };
        return myStats;
    }

    public void takesDamage(int amount)
    {
        health.setCurrent(health.getCurrent() - amount);
    }

    public Stat getHealth()
    {
        return health;
    }
    public int getHealthMax()
    {
        return health.getMax();
    }
    public int getHealthMin()
    {
        return health.getMin();
    }
    public int getHealthCurrent()
    {
        return health.getCurrent();
    }
    public void setHealth(Stat health)
    {
        this.health = health;
    }
    public void setHealthMax(int max)
    {
        this.health.setMax(max);
    }
    public void setHealthCurrent(int current)
    {
        this.health.setCurrent(current);
    }


    public Stat getSkill()
    {
        return skill;
    }
    public int getSkillMax()
    {
        return skill.getMax();
    }
    public int getSkillMin()
    {
        return skill.getMin();
    }
    public int getSkillCurrent()
    {
        return skill.getCurrent();
    }
    public void setSkill(Stat a)
    {
        this.skill = a;
    }
    public void setSkillMax(int max)
    {
        this.skill.setMax(max);
    }
    public void setSkillCurrent(int current)
    {
        this.skill.setCurrent(current);
    }


    public Stat getMomentum()
    {
        return momentum;
    }
    public int getMomentumMax()
    {
        return momentum.getMax();
    }
    public int getMomentumMin()
    {
        return momentum.getMin();
    }
    public int getMomentumCurrent()
    {
        return momentum.getCurrent();
    }
    public void setMomentum(Stat momentum)
    {
        this.momentum = momentum;
    }
    public void setMomentumMax(int max)
    {
        momentum.setMax(max);
    }
    public void setMomentumCurrent(int current)
    {
        motive.setCurrent(current);
    }




    public Stat getMeans()
    {
        return means;
    }
    public int getMeansMax()
    {
        return means.getMax();
    }
    public int getMeansMin()
    {
        return means.getMin();
    }
    public int getMeansCurrent()
    {
        return means.getCurrent();
    }
    public void setMeans(Stat means)
    {
        this.means = means;
    }
    public void setMeansMax(int max)
    {
        means.setMax(max);
    }
    public void setMeansCurrent(int current)
    {
        means.setCurrent(current);
    }




    public Stat getMotive()
    {
        return motive;
    }
    public int getMotiveMax()
    {
        return motive.getMax();
    }
    public int getMotiveMin()
    {
        return motive.getMin();
    }
    public int getMotiveCurrent()
    {
        return motive.getCurrent();
    }

    public void setMotive(Stat motive)
    {
        this.motive = motive;
    }
    public void setMotiveMax(int max)
    {
        motive.setMax(max);
    }
    public void setMotiveCurrent(int current)
    {
        motive.setCurrent(current);
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
