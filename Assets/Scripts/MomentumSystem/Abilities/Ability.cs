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