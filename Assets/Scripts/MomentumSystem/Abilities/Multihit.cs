using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Multihit", menuName = "RPG/Multihit", order = 2)]
    public class Multihit : Ability
    {
        //This class exists for multihit behaviors.
        [SerializeField] private int repeats;

        public int getRepeats()
        {
            return repeats;
        }
    }
}
