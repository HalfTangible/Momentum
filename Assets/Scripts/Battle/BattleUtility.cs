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

    public static StatSheet NextToFight(List<StatSheet> allParticipants)
    {
        //Sort all the StatSheets based on the Momentum of the participants.
        //Return the character with the highest momentum.
        //Actually, sorting them isn't necessary if you're just returning whoever's next.
        
        StatSheet nextToFight = allParticipants[0];
        /*
        for(int i = 1; i < allParticipants.Count; i++)
        {
            if (!nextToFight.isAlive())
                nextToFight = allParticipants[i];
            else
                if (nextToFight.momentum.Current < allParticipants[i].momentum.Current)
                nextToFight = allParticipants[i];
        }*/

        //Very inefficient
        //Get a list for the player party and the enemy party
        //Sort those two lists, with the dead at the very back, then compare the first person in each list.
        //It's a bit more annoying to code but it means we only have to sort the entirety of each array *once*.

        return nextToFight;


    }
}
