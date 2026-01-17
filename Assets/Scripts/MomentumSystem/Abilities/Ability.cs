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
        [SerializeField] private AbilityCategory category = AbilityCategory.Uncategorized;
        [SerializeField] private AbilityType type = AbilityType.Typeless;
        [SerializeField] private string abilityName;
        [SerializeField] private string description;
        [SerializeField] private int momentumCost;
        [SerializeField] protected List<ABehavior> behaviorList = new List<ABehavior>();


        // Start is called before the first frame update

        public AbilityCategory Category
        {
            get => category;
            set => category = value;
        }

        public AbilityType Type
        {
            get => type;
            set => type = value;
        }

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

        public void UseAbility1(StatSheet user, StatSheet target)
        {
            bool momentumPaid = false;


            foreach (ABehavior behavior in behaviorList)
            {
                bool beforeHit = (bool)behavior.GetStat<bool>("BEFOREHIT");
                bool onHit = (bool)behavior.GetStat<bool>("ONHIT");
                bool afterHit = (bool)behavior.GetStat<bool>("AFTERHIT");
                bool onUser = (bool)behavior.GetStat<bool>("ONUSER");
                bool overwhelming = false;

                StatSheet affected = onUser ? user : target;

                if (beforeHit && onUser)
                    behavior.Affects(affected);

                Debug.Log($"Ability: {abilityName} is checking Overwhelm!");
                overwhelming = BattleUtility.CheckOverwhelm(user, target); //This is done here in case any beforeHit effects would change the Overwhelm check.
                Debug.Log($"Ability: Overwhelming? {overwhelming}");

                //We'll need to change Affects so that it checks Overwhelming and has an effect

                if (onHit)
                    behavior.Affects(affected);

                if (!momentumPaid)
                {
                    user.SpendMomentum(momentumCost);
                    momentumPaid = true;
                }

                if (afterHit)
                    behavior.Affects(affected);


                if (behavior.Continues())
                    affected.AbilityHit(behavior);
                else
                    behavior.Finished(affected);
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

    public enum AbilityCategory
    {
        Offensive,
        Defensive,
        Uncategorized
    }

    public enum AbilityType
    {
        Physical,
        Magical,
        Skill,
        Typeless
    }

}