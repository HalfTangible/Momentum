using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Spell", menuName = "RPG/Spell", order = 3)]
    public class Spell : Ability
    {
        //This class exists for unique behaviors related to spells.
        //Spells are powerful abilities, but they take time to cast.
        //When they go off, they go off at their original Momentum, but for that time, the caster will be unable to act.
        //Possibly overpowered?
        //The spell's delay will be directly tied to the momentum cost.
        int delay;

        public void WhileCasting()
        {

        }

        public void FinishCasting()
        {

        }
    }
}
