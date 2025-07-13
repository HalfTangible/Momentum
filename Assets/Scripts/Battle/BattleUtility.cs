using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

public static class BattleUtility
{
    public static bool CheckOverwhelm(StatSheet user, StatSheet target)
    {
        //user momentum + skill >= target momentum + skill * 2
        return ((user.momentum.Current + user.skill.Current) >= (target.momentum.Current + (target.skill.Current * 2)));
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
            return RoundRefresh(statSheets); // Repeat until all have momentum > 0
        ApplyRoundEffects(statSheets); // Apply effects after all momentum refreshed
        return true; // Indicate round is complete
    }

    private static void ApplyRoundEffects(List<StatSheet> statSheets)
    {
        foreach (StatSheet sheet in statSheets)
        {
            if (sheet.isAlive())
                sheet.ApplyRoundEffects();
        }
    }
}
