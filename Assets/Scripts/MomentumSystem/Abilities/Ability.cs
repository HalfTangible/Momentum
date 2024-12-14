using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Ability", menuName = "RPG/Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        [SerializeField] private string abilityName;
        [SerializeField] private string description;
        [SerializeField] protected List<ABehavior> behaviors = new List<ABehavior>();

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

        public Ability()
        {
        
        }

/*        public Ability(string name)
        {
            Name = name;
        }
*/
        void OnHit(StatSheet user, StatSheet target)
        {
            //What these abilities do on hit
            //First, determine if the behaviors Overwhelm.
            //if(user.momentum + user.skill >= target.momentum + target.skill + target.skill
            //then add the user's overwhelming behavior to the ability

            target.AbilityHit(behaviors);
        }

        bool Finished()
        {
            foreach (var behavior in behaviors)
            {
                if (behavior.Continues())
                return false;
            }
            return true;
        }

        public List<ABehavior> GetBehaviors()
        {
            return behaviors;
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