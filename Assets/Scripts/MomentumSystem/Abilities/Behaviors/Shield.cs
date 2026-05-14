using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;
//using UnityEditor;

namespace RPG.AbilitySystem
{
    //[DisplayName("Shield - Tempory health")]
    [System.Serializable]
    public class Shield : ABehavior
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

            target.AddShield(amount);
            base.Affects(target);
        }

        public override void Overwhelms(StatSheet target)
        {
            Affects(target);
        }

        public override void Initialize(int amount)
        {
            Initialize(amount, true);
        }

        public void Initialize(int amount, bool onHit)
        {
            //By default, damage will happen once on hit.
            Initialize(amount, onHit, 0, 0);


        }

        public void Initialize(int amount, bool onHit, int rounds, int turns)
        {

            base.InitializeStats(amount, onHit, rounds, turns);

        }

        private string Description()
        {
            
            string desc = "Grants temporary shielding to absorb damage. \n";
            
            int amt = getAmount();
            bool onHit = actsOnHit();
            int rounds = getRounds();
            int turns = getTurns();

            if (onHit)
                desc += $"* Apply {amt} shielding on hit. \n";
            if (rounds == 1)
                desc += $"* Apply {amt} shielding at the start of the next round. \n";
            if (turns == 1)
                desc += $"* Apply {amt} shielding at the start of the target's next turn. \n";
            if (rounds > 1)
                desc += $"* Apply {amt} shielding at the start of the next round for {rounds} rounds. \n";
            if (turns > 1)
                desc += $"* Apply {amt} shielding at the start of the target's next turn for {turns} turns. \n";

            return desc.TrimEnd();
            
        }
    }
}