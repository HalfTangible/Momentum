using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.AbilitySystem
{
    public abstract class Ability
    {
        public string Name { get; private set; }
        protected List<IBehavior> behaviors = new List<IBehavior>();

        // Start is called before the first frame update

        public Ability(string name)
        {
            Name = name;
        }

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

    }
}