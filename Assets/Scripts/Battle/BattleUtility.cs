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

    public static void RoundRefresh(List<StatSheet> statSheets, List<StatSheet> statSheets2)
    {
        RoundRefresh(statSheets);
        RoundRefresh(statSheets2);
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

    public static StatSheet NextToFight(List<StatSheet> playerParty, List<StatSheet> enemyParty)
    {
        //Sort all the StatSheets based on the Momentum of the participants.
        //Return the character with the highest momentum.
        //Actually, sorting them isn't necessary if you're just returning whoever's next.

        SortParty(playerParty);
        SortParty(enemyParty);


        // Get the first living character from each party
        StatSheet player = playerParty.Find(p => p.isAlive());
        StatSheet enemy = enemyParty.Find(e => e.isAlive());

        // If no living players or enemies, return null
        if (player == null && enemy == null)
            return null;
        if (player == null)
            return enemy;
        if (enemy == null)
            return player;

        // Compare momentum of the first living characters
        return player.momentum.Current >= enemy.momentum.Current ? player : enemy;


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
