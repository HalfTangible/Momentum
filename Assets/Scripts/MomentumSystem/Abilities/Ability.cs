using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using Debug = UnityEngine.Debug;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "RPG/Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        [SerializeField] private string abilityName;
        [SerializeField] private string description;
        [SerializeField] private int momentumCost;
        [SerializeField] protected List<ABehavior> behaviorList = new List<ABehavior>();

        // Start is called before the first frame update

        public string AbilityName
        {
            get => abilityName;
            set => abilityName = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public int MomentumCost
        {
            get => momentumCost;
            set => momentumCost = value;
        }

        public Ability()
        {
        
        }

        public void BeforeHit(StatSheet user, StatSheet target)
        {
            foreach (ABehavior behavior in behaviorList)
            {
                if (behavior.GetStat<bool>("BEFOREHIT") == true)
                {
                    behavior.Apply(target);
                }
            }
        }

        public void OnHit(StatSheet user, StatSheet target)
        {

            //What these abilities do on hit
            //First, determine if the behaviors Overwhelm.
            //if(user.momentum + user.skill >= target.momentum + target.skill + target.skill
            //then add the user's overwhelming behavior to the ability

            //int threshold = user.momentum.Current + user.skill.Current - (target.momentum.Current + target.skill.Current * 2);
            //If the user's skill and momentum are enough to overcome the target's skill and momentum, then the ability overwhelms
            bool overwhelming = BattleUtility.CheckOverwhelm(user, target);

            //For the purposes of the prototyping stage we will apply the behaviors a second time for free, which in this case means double damage.

            Debug.Log($"{user.characterName} attacks {target.characterName} with {this.abilityName}.");

            foreach (ABehavior behavior in behaviorList)
            {

                if (behavior.GetStat<bool>("ONHIT") == true)
                {

                    if ((bool)behavior.GetStat<bool>("ONUSER"))
                    {
                        user.AbilityHit(behavior);
                    }
                    else
                    {
                        target.AbilityHit(behavior);
                        Debug.Log($"Ability (after hit): {user.characterName}: {user.momentum.Current} + {user.skill.Current} vs {target.characterName}: {target.momentum.Current} + {target.skill.Current * 2}.");
                        if (overwhelming)
                        {
                            Debug.Log($"{user.characterName} overwhelms {target.characterName}!");

                            //Note: Check to see if this works with DOTS and the like; I don't know if I need to instantiate a new behaviors list or something.

                            target.AbilityHit(behavior);
                        }
                    }
                }
            }
            Debug.Log("Momentum: " + user.momentum.getValues());
            Debug.Log($"{user.characterName} is spending {momentumCost} momentum. Before: {user.momentum.Current}");
            user.SpendMomentum(momentumCost);
            Debug.Log($"{user.characterName} after spend: {user.momentum.Current}");

            foreach (ABehavior behavior in behaviorList)
            {
                Debug.Log($"{behavior.GetType().Name}: {behavior.Continues()}");
                Debug.Log($"Continues?: {!behavior.Continues()}");
                if (!behavior.Continues())
                {
                    Debug.Log($"Finished called for {behavior.GetType().Name}");
                    behavior.Finished(target);
                }
            }

            Debug.Log($"{user.characterName} after OnHit: {user.momentum.Current}");
            Debug.Log($"{user.momentum.getValues()}");

        }

        public void AfterHit(StatSheet user, StatSheet target)
        {
            foreach (ABehavior behavior in behaviorList)
            {
                if (behavior.GetStat<bool>("AFTERHIT") == true)
                {
                    behavior.Apply(target);
                }
            }
        }


        bool Finished()
        {
            foreach (var behavior in behaviorList)
            {
                if (behavior.Continues())
                return false;
            }
            return true;
        }

        public List<ABehavior> GetBehaviors()
        {
            return behaviorList;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetName(string newName)
        {
            this.name = newName;
        }

        public string GetAbilityName()
        {
            return abilityName;
        }

        public void SetAbilityName(string abilityName)
        {
            this.abilityName = abilityName;
        }

    }
}