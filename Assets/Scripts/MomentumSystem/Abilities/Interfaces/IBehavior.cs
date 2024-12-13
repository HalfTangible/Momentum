using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{

    public interface IBehavior
    {
        //void Apply(StatSheet user, StatSheet target);
        

        bool Continues();
        bool Finished();
        void OnHit(StatSheet target);

    }

}