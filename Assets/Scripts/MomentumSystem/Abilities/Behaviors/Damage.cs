using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StatSystem;

namespace RPG.AbilitySystem
{
    public class Damage : IBehavior
    {
        int amount;
        int roundsRemaining;
        int turnsRemaining;
        bool onHit;

        /*public void Apply(StatSheet user, StatSheet target)
        {
            //Attaches this behavior to the target. It then triggers based on
            //target.AbilityHit(this);

            //Actually... apply would need to be used by the ability, wouldn't it?
        }*/

        public bool EachTurn(StatSheet target)
        {
            if(turnsRemaining > 0)
            {
                target.TakesDamage(amount);
                turnsRemaining--;
            }
            
            return Continues();
        }

        public bool EachRound(StatSheet target)
        {
            if (roundsRemaining > 0)
            {
                target.TakesDamage(amount);
                roundsRemaining--;
            }
            
            return Continues();
        }

        public bool Continues()
        {
            //Called at the end of apply. The code checks to see if the ability is done.
            if (roundsRemaining > 0 || turnsRemaining > 0)
                return true; else return false;
        }

        public bool Finished() 
        {
            //This behavior is called when the ability's behaviors are done.
            //If this is a buff, then it removes the buff; debuff, same deal.
            //Since this behavior is damage, we don't need to do that, just be done.
            return false;
        }
        
        public void OnHit(StatSheet target)
        {
            if (onHit)
                target.TakesDamage(amount);
            //With the OnHit done, we check to see if the effect continues.
        }

        public Damage(int amountEachTick) : this(amountEachTick, true)
        {

        }

        public Damage(int amountEachTick, bool onHit)
        {
            amount = amountEachTick;
            this.onHit = onHit;
        }

        public Damage(int amountEachTick, bool onHit, int rounds, int turns) : this(amountEachTick, onHit) { 
        
            roundsRemaining = rounds;
            turnsRemaining = turns;
                
        }
    }
}