using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    [DisplayName("Ward - Nullify ability")]
    [System.Serializable]
    public class Ward : ABehavior
    {
        public override bool EachTurn(StatSheet target)
        {
            int turnsRemaining = (int)GetStat<int>("TURNS");

            if (turnsRemaining > 0)
            {
                Affects(target);
                SetStat("TURNS", --turnsRemaining);
            }

            return Continues();
        }

        public override bool EachRound(StatSheet target)
        {
            int roundsRemaining = (int)GetStat<int>("ROUNDS");

            if (roundsRemaining > 0)
            {
                Affects(target);
                SetStat("ROUNDS", --roundsRemaining);
            }

            return Continues();
        }


        public override void Finished(StatSheet target)
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            base.Finished(target);
        }

        public override void Affects(StatSheet target)
        {
            target.AddWard(amount);

            base.Affects(target);

            //With the OnHit done, we check to see if the effect continues.
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

            string desc = "Damaging ability. \n";
            /*
            int amount = (int)GetStat<int>("AMOUNT");
            bool onHit = (bool)GetStat<bool>("ONHIT");
            int rounds = (int)GetStat<int>("ROUNDS");
            int turns = (int)GetStat<int>("TURNS");

            if (onHit)
                desc += $"*On hit, do {amount} damage.\n";
            if (rounds == 1)
                desc += $"*Do {amount} damage at the start of the next round.";
            if (turns == 1)
                desc += $"Do {amount} damage at the start of the target's next turn.";
            if (rounds > 1)
                desc += $"*Do {amount} damage at the start of each round for {rounds} rounds";
            if (turns > 1)
                desc += $"Do {amount} damage each turn at the start of each of the target's turns for {turns} turns.";
            */

            return desc;

        }
    }
}