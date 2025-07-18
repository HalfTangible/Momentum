using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

public static class BattleUtility
{
    public static bool CheckOverwhelm(StatSheet user, StatSheet target)
    {
        //user momentum + skill >= target momentum + skill * 2
        Debug.Log("BattleUtility: Check Overwhelm!");
        Debug.Log($"{user.characterName} threshold: {user.momentum.Base} + {user.momentum.Buff} - {user.momentum.Debuff} + Skill: {user.skill.Current} ");
        Debug.Log($"{target.characterName}: {target.momentum.Base} + {target.momentum.Buff} - {target.momentum.Debuff} + {target.skill.Current * 2}.");
        return ((user.momentum.Current + user.skill.Current) >= (target.momentum.Current + (target.skill.Current * 2)));
    }

    public static bool RoundRefreshCheck(List<StatSheet> statSheets)
    {
        foreach (StatSheet sheet in statSheets)
            if (sheet.momentum.Current <= 0 && sheet.isAlive())
                return true;

        return false;
    }

    public static bool RoundRefresh(List<StatSheet> statSheets)
    {
        bool momentumPending = false;

        foreach (StatSheet sheet in statSheets)
        {
            if (sheet.isAlive())
            {
                sheet.RefreshMomentum();
                if (sheet.momentum.Current <= 0)
                    momentumPending = true;
            }
        }
        
        if (momentumPending)
            return false; ; // Indicate another refresh is needed
        
        
        return true; // Indicate round is complete
    }

    public static void ApplyRoundEffects(List<StatSheet> statSheets)
    {
        foreach (StatSheet sheet in statSheets)
        {
            if (sheet.isAlive())
                sheet.ApplyRoundEffects();
        }
    }

    public static void SortParty(List<StatSheet> party)
    {
        party.Sort((a, b) =>
        {
            // Dead characters go to the end
            if (!a.isAlive() && !b.isAlive()) return 0; // Both dead, no change
            if (!a.isAlive()) return 1; // a dead, b alive, a goes after
            if (!b.isAlive()) return -1; // b dead, a alive, a goes before
            // Both alive, sort by momentum descending
            return b.momentum.Current.CompareTo(a.momentum.Current);
        });
    }
}
