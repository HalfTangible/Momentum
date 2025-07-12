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

    public static bool RoundRefresh(StatSheet theCharacter)
    {
        //Check if the statsheet is alive.
        //If so, return false.

        if(theCharacter.momentum.Current <= 0)
            return false;

        //int newRoundMomentum = a.getMomentumCurrent() + a.getMotiveCurrent();

        theCharacter.momentum.Current += theCharacter.motive.Current;
        return true;
    }

    public static bool RoundRefresh(List<StatSheet> statSheets)
    {
        bool isAnyoneAlive = false;

        foreach (StatSheet characterSheet in statSheets)
            if (RoundRefresh(characterSheet))
                isAnyoneAlive = true;

            

        return isAnyoneAlive;
    }
}
