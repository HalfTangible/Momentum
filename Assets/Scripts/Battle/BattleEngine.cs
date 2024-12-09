using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlephase;
using RPG.AbilitySystem;

public class BattleEngine : MonoBehaviour
{


    BattlePhase currentPhase;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {
        currentPhase = BattlePhase.Setup;
    }

    public void WhosNext(StatSheet player, StatSheet npc)
    {
        //Check the sheets of our two combatants (for now; later we'll make it so it returns the next person in each party).
        //If one of these characters is dead, then that side loses.
        if (player.getHealthCurrent() <= 0)
            PlayerLose();
        else if (npc.getHealthCurrent() <= 0)
            PlayerWin();

        //If either the player or the NPC are at 0 Momentum, then the round ends and a new one begins that refreshes their momentum.
        if (player.getMomentumCurrent() == 0 || npc.getMomentumCurrent() == 0)
            NewRound();

        //The sheet with the higher Momentum goes next.

        if (npc.getMomentumCurrent() > player.getMomentumCurrent())
            currentPhase = BattlePhase.NonPlayerTurn;
        else if (player.getMomentumCurrent() > npc.getMomentumCurrent())
            currentPhase = BattlePhase.PlayerTurn;
    }

    public void PlayerTurn(StatSheet player)
    {
        //Player selects an action and spends Momentum.
    }

    public void NonPlayerTurn(StatSheet npc)
    {
        //Eventually we'll want to assign viable attacks and weights to decide what ability each character uses.
        //For now, just have them select heavy or basic attacks at random.
    }

    public void NewRound()
    {
        //Give all stat sheets in the current battle a refresh on their momentum.
    }

    public void PlayerWin()
    {

    }

    public void PlayerLose()
    {

    }

    public void Finish()
    {

    }
}

namespace Battlephase
{
    [System.Serializable]
    public enum OnPhase { OnHit, WhenHit, StartTurn, EndTurn, StartRound, EndRound, Permanent }
    
    public enum BattlePhase { Setup, PlayerTurn, NonPlayerTurn, WhosNext, NewRound, Finish}
}
