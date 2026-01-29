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

        public int wards;
        public int counters;
        public int shield;

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

        public void AddWard(int amount)
        {
            wards += amount;
        }

        public void RemoveWard()
        {
            wards--;
        }

        public void AddShield(int amount)
        {
            shield += amount;
        }

        public void AddCounter(int amount)
        {
            counters += amount;
        }

        public void AbilityHit(Ability ability)
        {
            AbilityHit(ability.GetBehaviors());
        }

        public void AbilityHit(List<ABehavior> input)
        {
            foreach (ABehavior behavior in input)
            {
                AbilityHit(behavior);
            }
        }

        public int ApplyDefenses(int incomingDamage)
        {
            Debug.Log($"{characterName}.ApplyDefenses; incomingDamage = {incomingDamage}");
            Debug.Log($"Wards: {wards}");

            if (wards > 0)
            {
                wards--;
                Debug.Log($"Ward applied. {wards} left.");
                return 0;
            }

            int damageAfterDefenses = incomingDamage;
            Debug.Log($"Shields: {shield}");
            //Then shielding absorbs what's left
            if (shield >= damageAfterDefenses && damageAfterDefenses > 0)
            {
                shield -= damageAfterDefenses;
                Debug.Log($"Shields left: {shield}, Damage going through: 0");
                return 0;
            }
            else if (shield < damageAfterDefenses && shield > 0)
            {
                damageAfterDefenses -= shield;
                Debug.Log($"Shields left: 0, Shields left: {shield}");
                shield = 0;
            }

            Debug.Log($"Before {continuingEffects.Count} continuingEffects: {damageAfterDefenses}");

            foreach (ABehavior behavior in continuingEffects)
            {
                Debug.Log($"{behavior.name}");
                // Safe type check + cast
                if (behavior is Grit gritBehavior && damageAfterDefenses > 1)
                {
                    // Assuming Grit has a public field/property like this
                    damageAfterDefenses = behavior.ModifyIncomingDamage(damageAfterDefenses); 
                    Debug.Log($"Grit reduced damage. New total: {damageAfterDefenses}");
                }

                // Exit early if we've reduced it to 1 or below
                if (damageAfterDefenses <= 1)
                {
                    damageAfterDefenses = 1;  // Optional: enforce minimum here
                    break;                    // Stops looping — no need to check more Grit behaviors
                }
                //Later: We're going to have Wards in the game as a behavior. We might decide later to have them only trigger against certain levels of damage to make their
                //shielding effect more potent. If so, we'll put in a check for Ward behavior here.
            }
            Debug.Log($"After all behaviors, total is: {damageAfterDefenses}");
            return damageAfterDefenses;
            //If counters exist, we can trigger them elsewhere; this is for calculating damage.
        }

        public void AbilityHit(ABehavior behavior)
        {
            //Reminder: need to account for things like buff and debuff which are OnHit and end with the turn.
            //Can just have a check at the end of the turn that removes the buff/debuff and then add them, yeah?
            Debug.Log($"{behavior.name} hits {characterName}. Continues? {behavior.Continues()} Turns: { behavior.getTurns()}, Rounds: { behavior.getRounds()}");
            
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