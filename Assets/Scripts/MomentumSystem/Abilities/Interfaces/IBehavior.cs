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
        //private Dictionary<string, object> stats;
        public List<string> GetKeys();
        public T GetStat<T>(string key);
        public void SetStat(string key, object value);


    }

}