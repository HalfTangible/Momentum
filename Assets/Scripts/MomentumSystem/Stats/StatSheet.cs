using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.AbilitySystem;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RPG.StatSystem
{
    [System.Serializable]
    public class StatSheet
    {
        public Stat health;
        public Stat momentum;
        public Stat motive;
        public Stat means;
        public Stat skill;

        [SerializeField] private List<Ability> abilities;
        [SerializeField] private List<ABehavior> continuingEffects;
        public string characterName;

        public StatSheet()
        {
            abilities = new List<Ability>();
            continuingEffects = new List<ABehavior>();

            health = new Stat(100, StatType.Resource) { Name = "HEALTH" };
            momentum = new Stat(10, StatType.Unbounded) { Name = "MOMENTUM" };
            motive = new Stat(10, StatType.Character) { Name = "MOTIVE" };
            means = new Stat(10, StatType.Character) { Name = "MEANS" };
            skill = new Stat(3, StatType.Character) { Name = "SKILL" };
        }

        public void AddAbility(Ability newAbility) => abilities.Add(newAbility);
        public void AddAbilities(List<Ability> newAbilities) => abilities.AddRange(newAbilities);
        public void RemoveAbility(Ability oldAbility) => abilities.Remove(oldAbility);
        public List<Ability> GetAbilities() => abilities;

        public void AbilityHit(List<ABehavior> input)
        {
            foreach (ABehavior behavior in input)
            {
                AbilityHit(behavior);
            }
        }

        public void AbilityHit(ABehavior behavior)
        {
            //Reminder: need to account for things like buff and debuff which are OnHit and end with the turn.
            //Can just have a check at the end of the turn that removes the buff/debuff and then add them, yeah?

            if (behavior.Continues()) continuingEffects.Add(behavior);
        }

        public void SpendMomentum(int amount) => momentum.Current -= amount;
        public void TakesDamage(int amount) => health.Current -= amount;
        public void Heals(int amount) => health.Current += amount;
        public void Buffs(string statName, int amount) => GetStatByName(statName)?.ApplyBuff(amount);
        public void Debuffs(string statName, int amount) => GetStatByName(statName)?.ApplyDebuff(amount);
        public bool isAlive() => health.Current > 0;

        public Stat[] GetStats() => new[] { health, momentum, motive, means, skill };

        public List<string> GetStatNames()
        {
            List<string> allStatNames = new List<string>();
            foreach (Stat stat in GetStats())
            {
                allStatNames.Add(stat.Name);
            }
            return allStatNames;
        }

        public Stat GetStatByName(string statName)
        {
            foreach (Stat stat in GetStats())
            {
                if (statName == stat.Name) return stat;
            }
            return null;
        }

        public void RefreshMomentum()
        {
            if (health.Current <= 0) return;
            momentum.Current += motive.Current;
        }
        //EachTurn and EachRound in the behavior already sends back whether it's done or not.
        public void ApplyRoundEffects()
        {
            if (health.Current <= 0) return;
            List<ABehavior> toRemove = new List<ABehavior>();
            foreach (ABehavior behavior in continuingEffects)
            {
                if (!behavior.EachRound(this)) //EachRound returns a check to see if it's done.
                    toRemove.Add(behavior);
                
            }
            foreach (ABehavior behavior in toRemove)
            {
                behavior.Finished(this);
                continuingEffects.Remove(behavior);
            }
        }

        public void ApplyTurnEffects()
        {
            if (health.Current <= 0) return;
            List<ABehavior> toRemove = new List<ABehavior>();
            foreach (ABehavior behavior in continuingEffects)
            {
                if (!behavior.EachTurn(this)) //EachTurn returns a check to see if it's done.
                    toRemove.Add(behavior);
            }
            foreach (ABehavior behavior in toRemove)
            {
                behavior.Finished(this);
                continuingEffects.Remove(behavior);
            }
        }
    }


}