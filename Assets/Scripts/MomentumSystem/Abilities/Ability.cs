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

        public int MomentumCost
        {
            get => momentumCost;
            set => momentumCost = value;
        }

        public Ability()
        {
        
        }

/*        public Ability(string name)
        {
            Name = name;
        }
*/
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

            target.AbilityHit(behaviors);
            if (overwhelming){
                Debug.Log($"{user.characterName} overwhelms {target.characterName}!");

                //Note: Check to see if this works with DOTS and the like; I don't know if I need to instantiate a new behaviors list or something.

                target.AbilityHit(behaviors);
            }

            user.SpendMomentum(momentumCost);

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