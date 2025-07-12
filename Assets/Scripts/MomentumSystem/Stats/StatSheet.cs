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

            health = new Stat(10, StatType.Resource) { Name = "HEALTH" };
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
            foreach (ABehavior a in input)
            {
                a.OnHit(this);
                if (a.Continues()) continuingEffects.Add(a);
            }
        }

        public void SpendMomentum(int amount) => momentum.Current -= amount;
        public void TakesDamage(int amount) => health.Current -= amount;
        public void Heal(int amount) => health.Current += amount;
        public void Buff(string statName, int amount) => GetStatByName(statName)?.ApplyBuff(amount);
        public void Debuff(string statName, int amount) => GetStatByName(statName)?.ApplyDebuff(amount);
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

        public void NewRound()
        {
            if (health.Current > 0)
            {
                momentum.Current = momentum.Current + motive.Current;
            }
        }
    }
}