using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

public static class BattleUtility
{
    public static bool CheckOverwhelm(StatSheet user, StatSheet target)
    {
        //user momentum + skill >= target momentum + skill * 2
        return ((user.GetMomentumCurrent() + user.GetSkillCurrent()) >= (target.GetMomentumCurrent() + (target.GetSkillCurrent() * 2)));
    }

    public static bool RoundRefresh(StatSheet a)
    {
        //Check if the statsheet is alive.
        //If so, return false.

        if(a.GetHealthCurrent() <= 0)
            return false;

        //int newRoundMomentum = a.getMomentumCurrent() + a.getMotiveCurrent();

        a.SetMomentumCurrent(a.GetMomentumCurrent() + a.GetMotiveCurrent());
        return true;
    }

    public static bool RoundRefresh(List<StatSheet> statSheets)
    {

        foreach (StatSheet s in statSheets)
            RoundRefresh(s);

        return true;
    }
}
