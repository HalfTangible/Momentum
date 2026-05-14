using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
//using UnityEditor;

namespace RPG.AbilitySystem
{
    //[DisplayName("Ward - Nullify ability")]
    [System.Serializable]
    public class Ward : ABehavior
    {
        public override bool EachTurn(StatSheet target)
        {
            int turnsRemaining = getTurns();

            if (turnsRemaining > 0)
            {
                Affects(target);
                SetStat("TURNS", turnsRemaining - 1);
            }

            return Continues();
        }

        public override bool EachRound(StatSheet target)
        {
            int roundsRemaining = getRounds();

            if (roundsRemaining > 0)
            {
                Affects(target);
                SetStat("ROUNDS", roundsRemaining - 1);
            }

            return Continues();
        }

        public override void Affects(StatSheet target)
        {
            target.AddWard(amount);
            base.Affects(target);
        }

        public override void Overwhelms(StatSheet target)
        {
            Affects(target);
        }

        public override void Initialize(int amount)
        {
            Initialize(amount, true); // Default: apply on hit
        }

        public void Initialize(int amount, bool onHit)
        {
            Initialize(amount, onHit, 0, 0); // Default: do not continue on next turn or next round
            //Why would you ever make a shield amount with no turn or rounds?
            //Question for later. Get it all working right now then worry about efficiency.
        }

        public void Initialize(int amount, bool onHit, int rounds, int turns)
        {

            base.InitializeStats(amount, onHit, rounds, turns);

        }

        private string Description()
        {
            //This should only be called when an Ability's description is being constructed.

            string desc = "Creates a ward. A ward nullifies one ability that hits the target. \n";
            
            int amount = getAmount();
            bool onHit = actsOnHit();
            int rounds = getRounds();
            int turns = getTurns();

            if (onHit)
                desc += $"* Apply {amount} warding on hit. \n";
            if (rounds == 1)
                desc += $"* Apply {amount} warding at the start of the next round.";
            if (turns == 1)
                desc += $"* Apply {amount} warding at the start of the target's next turn.";
            if (rounds > 1)
                desc += $"* Apply {amount} warding at the start of the next round for {rounds} rounds.";
            if (turns > 1)
                desc += $"* Apply {amount} warding at the start of the target's next turn for {turns} turns.";
            
            return desc.TrimEnd();

        }
    }
}