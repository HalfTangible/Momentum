using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RPG.AbilitySystem
{
    [System.Serializable]
    public class Buff : ABehavior
    {
        //This class needs to be able to distinguish between valid targets?
        //No, that would probably be a thing the main ability would need to do.
        //It does, however, need to select a stat to buff. Which means we need a target stat; both one to select for the buff, and one to
        
        [SerializeField]
        List<string> buffTargets; //For the AbilityEditor's benefit; we need to select which stat we're targetting
        [SerializeField]
        string targetStat; //This will be what stores the target stat for runtime.
        [SerializeField]
        int targetStat_index = 0;
        int amountApplied = 0;

        // https://x.com/HalfTangible/status/2009771283555156221

        public Buff()
        {
            StatSheet temp = new StatSheet();
            buffTargets = temp.GetStatNames();
            buffTargets.Remove("HEALTH");
            allKeys.AddRange(new[] {"BUFFTARGETS", "TARGETSTAT"});
            allKeys.Sort();
        }

        public override void Affects(StatSheet target)
        {
            Debug.Log($"[Buff.Affects] Applying to {target.characterName}'s {targetStat}: amount = {amount} (positive = buff)");

            if (target.GetStatByName(targetStat) != null)
            {
                target.GetStatByName(targetStat).ApplyBuff(amount);
                amountApplied += amount;
                base.Affects(target);
            }
            else
                Debug.LogWarning($"Buff.Affects: Invalid target stat '{targetStat}' for {target.characterName}");
        }

        public override void Overwhelms(StatSheet target)
        {
            Affects(target);
        }


        public override void Finished(StatSheet target)
        {
            Debug.Log("Buff.Finished() called.");
            if (target.GetStatByName(targetStat) != null)
            {
                Debug.Log($"Start of Finished: {target.GetStatByName(targetStat).Buff}");
                target.GetStatByName(targetStat).ApplyBuff(amountApplied * -1);
                amountApplied = 0;
                base.Finished(target);
                Debug.Log($"Finished: {target.GetStatByName(targetStat).Buff}");
            }
            else
                Debug.LogWarning($"Buff.OnHit: Invalid target stat '{targetStat}' for {target.characterName}");

            Debug.Log(target.GetStatByName(targetStat).getValues());
        }

        public override object GetStat<T>(string key)
        {
            //UnityEngine.Debug.Log($"A_Key: '{key}' (Length: {key.Length})");
            //UnityEngine.Debug.Log($"A_Expected Key: 'BUFFTARGETS' (Length: {"BUFFTARGETS".Length})");
            //UnityEngine.Debug.Log($"A_Key.Equals('BUFFTARGETS'): {key.Equals("BUFFTARGETS")}");

           
            key = key.Trim().ToUpper();

            switch (key)
            {
                case "BUFFTARGETS":
                    //UnityEngine.Debug.Log("A_Key matched BUFFTARGETS");
                    return buffTargets;
                case "TARGETSTAT":
                    targetStat = buffTargets[targetStat_index];
                    return targetStat;
                case "TARGETSTAT_INDEX":
                    return targetStat_index;
                default:
                    return base.GetStat<T>(key);


            }
        }

        public override int GetListIndex(string key)
        {
            switch (key)
            { 
                case "BUFFTARGETS":
                    //UnityEngine.Debug.Log("B_Key matched BUFFTARGETS");
                    return targetStat_index;
            }
            return base.GetListIndex(key);
        }

        public override string GetListKey(string key) //Used to retrieve the key to the variable used to store the value of a given list
        {
            //UnityEngine.Debug.Log($"B_Key: '{key}' (Length: {key.Length})");
            //UnityEngine.Debug.Log($"B_Expected Key: 'BUFFTARGETS' (Length: {"BUFFTARGETS".Length})");
            //UnityEngine.Debug.Log($"B_Key.Equals('BUFFTARGETS'): {key.Equals("BUFFTARGETS")}");

            key = key.Trim().ToUpper();

            switch (key)
            {
                case "BUFFTARGETS":
                    //UnityEngine.Debug.Log("B_Key matched BUFFTARGETS");
                    return "TARGETSTAT";
            }

            UnityEngine.Debug.Log($"B_GetListKey in Buff doesn't recognize {key}");


            return base.GetListKey(key);
        }

        public override void SetStat(string key, List<string> values)
        {
            UnityEngine.Debug.Log($"C_SetStat {key}, {values}");

            switch (key)
            {
                /*
                case "BUFFTARGETS":
                    break;
                case "TARGETSTATS"
                */
                default:
                    base.SetStat(key, values);
                    break;
            }
        }

        public override void SetStat(string key, object value)
        {
            //UnityEngine.Debug.Log($"D_SetStat {key}, {value}");

            switch (key)
            {
                case "BUFFTARGETS": //If you're setting this, it's because you're changing the entry in a list.
                    targetStat_index = (int) value;
                    break;
                case "TARGETSTAT":
                    targetStat = (string) value;
                    break;
                case "TARGETSTAT_INDEX":
                    targetStat_index = (int) value;
                    break;
                default:
                    base.SetStat(key, value);
                    break;
            }
        }


    }
}