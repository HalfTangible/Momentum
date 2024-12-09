using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleUtility
{
    public static bool CheckOverwhelm(StatSheet user, StatSheet target)
    {
        //user momentum + skill >= target momentum + skill * 2
        return ((user.getMomentumCurrent() + user.getSkillCurrent()) >= (target.getMomentumCurrent() + (target.getSkillCurrent() * 2)));
    }

    public static bool RoundRefresh(StatSheet a)
    {
        //Check if the statsheet is alive.
        //If so, return false.

        if(a.getHealthCurrent() <= 0)
            return false;

        //int newRoundMomentum = a.getMomentumCurrent() + a.getMotiveCurrent();

        a.setMomentumCurrent(a.getMomentumCurrent() + a.getMotiveCurrent());
        return true;
    }

    public static bool RoundRefresh(List<StatSheet> statSheets)
    {

        foreach (StatSheet s in statSheets)
            RoundRefresh(s);

        return true;
    }
}
