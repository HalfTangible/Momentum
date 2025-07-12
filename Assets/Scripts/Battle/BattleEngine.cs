using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlephase;
using RPG.AbilitySystem;
using RPG.StatSystem;
using RPG.Battle.UI;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


namespace RPG.Battle {
    public class BattleEngine : MonoBehaviour
    {

        
        BattlePhase currentPhase;
        BattleUI battleUI;
        StatSheet player;
        StatSheet npc;
        bool firstRound;
        private bool advanceTurn = false; //For manual stopping

        List<Ability> playerAbilities;
        List<Ability> npcAbilities;

        // Start is called before the first frame update
        void Start()
        {
            advanceTurn = false;
            battleUI = FindObjectOfType<BattleUI>();
            if (battleUI == null) Debug.LogError("BattleUI not found in scene!");

            // Initialize test characters
            player = new StatSheet { characterName = "Hero" };
            npc = new StatSheet { characterName = "Goblin" };

            Ability basicAttack = Resources.Load<Ability>("BasicAttack_Test");
            Ability heavyAttack = Resources.Load<Ability>("HeavyAttack_Test");

            player.AddAbility(Instantiate(basicAttack));
            player.AddAbility(Instantiate(heavyAttack));

            npc.AddAbility(Instantiate(basicAttack));
            npc.AddAbility(Instantiate(heavyAttack));

            Setup();
            battleUI?.RefreshUI(player, npc); // Initial UI update
            WhosNext(player, npc);           // Start the battle

        }

        // Update is called once per frame
        void Update()
        {

            // Stop if battle is finished
            if (currentPhase == BattlePhase.Finish)
            {
                battleUI?.RefreshUI(player, npc); // Final update
                return;
            }

            // Advance from Waiting only on button press
            if (currentPhase == BattlePhase.Waiting && advanceTurn)
            {
                advanceTurn = false;
                WhosNext(player, npc);
            }

            if (advanceTurn && currentPhase != BattlePhase.Finish)
            {
                advanceTurn = false;
                //Debug.Log($"At the switch: {currentPhase}");
                switch (currentPhase)
                {
                    case BattlePhase.PlayerTurn:
                        currentPhase = BattlePhase.Waiting;
                        PlayerTurn(player);
                        break;
                    case BattlePhase.NonPlayerTurn:
                        currentPhase = BattlePhase.Waiting;
                        NonPlayerTurn(npc);
                        break;
                    case BattlePhase.WhosNext:
                        WhosNext(player, npc);
                        break;
                    case BattlePhase.NewRound:
                        currentPhase = BattlePhase.Waiting;
                        NewRound();
                        break;

                }
            }
        }

        public void Setup()
        {
            currentPhase = BattlePhase.Setup;
            advanceTurn = false;
            //Debug.Log("Battle setup complete.");
        }

        public void WhosNext(StatSheet player, StatSheet npc)
        {
            //Debug.Log("WhosNext called");
            //This happens first because I just remembered end-of-round effects are a thing I want to do later.
            //If either the player or the NPC are at 0 Momentum, then the round ends and a new one begins that refreshes their momentum.
            if (player.momentum.Current <= 0 || npc.momentum.Current <= 0)
            {
                //Debug.Log("If1 in");
                NewRound();
            }

            //Debug.Log("If1 out");

            //Check the sheets of our two combatants (for now; later we'll make it so it returns the next person in each party).
            //If one of these characters is dead, then that side loses.
            if (player.health.Current <= 0)
            {

                //Debug.Log("If2a in");
                PlayerLose();
                return;
            }
            else if (npc.health.Current <= 0)
            {

                //Debug.Log("If2b in");
                PlayerWin();
                return;
            }


            //Debug.Log($"If2 out. Also: npc momentum = {npc.GetMomentumCurrent()} and player momentum = {player.GetMomentumCurrent()}");


            //The sheet with the higher Momentum goes next.

            if (npc.momentum.Current > player.momentum.Current)
            {

                //Debug.Log("If3A in");
                currentPhase = BattlePhase.NonPlayerTurn; }
            else if (player.momentum.Current > npc.momentum.Current)
            {

                //Debug.Log("If3B in");
                currentPhase = BattlePhase.PlayerTurn; 
            }
            else if (currentPhase != BattlePhase.PlayerTurn && currentPhase != BattlePhase.NonPlayerTurn)
            {
                currentPhase = BattlePhase.PlayerTurn; //I'd rather it be random but giving the player an edge is fine and it's also a test RN so who cares
                //Debug.Log("If3C in");
            }

            //Debug.Log("If3 out");
        }

        public void PlayerTurn(StatSheet player)
        {
            //Player selects an action and spends Momentum.
            // Placeholder: For now, just reduce Momentum and deal damage
            Debug.Log("Player's turn!");

            if (player.GetAbilities().Count > 0)
            {
                int temp = Random.Range(0, player.GetAbilities().Count);
                Ability selectedAbility = player.GetAbilities()[temp];
                selectedAbility.OnHit(player, npc);
            }
            else
            {
                Debug.LogWarning("NPC has no abilities (somehow)");
                return;
            }
            EndTurn();
        }

        public void NonPlayerTurn(StatSheet npc)
        {
            //Eventually we'll want to assign viable attacks and weights to decide what ability each character uses.
            //For now, just have them select heavy or basic attacks at random.

            Debug.Log("NPC's turn!");
            /*
            // Randomly pick "basic" (2 Momentum, 1 damage) or "heavy" (5 Momentum, 3 damage)
            bool useHeavy = Random.value > 0.5f && npc.GetMomentumCurrent() >= 5;
            int momentumCost = useHeavy ? 5 : 2;
            int damage = useHeavy ? 3 : 1;

            npc.SetMomentumCurrent(npc.GetMomentumCurrent() - momentumCost);
            player.TakesDamage(damage);*/


            //CONCERN: If the ability overwhelms once, will every instance of this ability from then on do Overwhelm?
            //SUGGESTION: Instantiate ability before sending it through OnHit? Then when the ability is done it destroys itself so we don't overuse memory?
            //ALT: On Overwhelm, instantiate a copy of the ability, then modify that copy's behaviors with the Overwhelming effects. That way we only burden the memory with
            //Overwhelming attacks and so long as we destroy the overwhelming abilities and their effects once they're done we won't have any problems.

            if (npc.GetAbilities().Count > 0)
            {
                int temp = Random.Range(0, npc.GetAbilities().Count);
                Ability selectedAbility = npc.GetAbilities()[temp];
                selectedAbility.OnHit(npc, player);
            }
            else
            {
                Debug.LogWarning("NPC has no abilities (somehow)");
                return;
            }

            EndTurn();
            
        }

        public void EndTurn()
        {
            
            currentPhase = BattlePhase.WhosNext;
            battleUI?.RefreshUI(player, npc); // Update UI after turn
        }

        public void NewRound()
        {

            Debug.Log("New round!");
            //Give all stat sheets in the current battle a refresh on their momentum.
            //Increase their momentum by their motive
            if (currentPhase == BattlePhase.Setup)
            {
                
            }
            else 
            {
                //Debug.Log($"Player: Momentum is {player.GetMomentumCurrent()}; motive is {player.GetMotiveCurrent()} ");
                player.NewRound(); //Player and NPC gain their Momentum for the round, based on Motive.
                //Debug.Log($"NPC: Momentum is {npc.GetMomentumCurrent()}; motive is {npc.GetMotiveCurrent()} ");
                npc.NewRound();
                //Debug.Log($"Player: Momentum is {player.GetMomentumCurrent()}; NPC is {npc.GetMomentumCurrent()} ");
            }
            
            //Debug.Log("New round started!");
            currentPhase = BattlePhase.WhosNext;
            battleUI?.RefreshUI(player, npc);
        }

        public void PlayerWin()
        {

            //Leave the battle and go to the victory screen, probably gain XP and stuff
            Debug.Log("Player wins!");
            currentPhase = BattlePhase.Finish;
        }

        public void PlayerLose()
        {
            //Leave the battle and go to the defeat screen
            Debug.Log("Player loses!");
            currentPhase = BattlePhase.Finish;
        }

        public void Finish()
        {
            Debug.Log("Battle ended.");
        }

        public BattlePhase GetCurrentPhase()
        {
            return currentPhase;
        }

        // Call this from a button to advance
        public void AdvanceTurn()
        {
            Debug.Log($"Phase is {currentPhase}");
            advanceTurn = true;
        }


    }
}

namespace Battlephase
{
    [System.Serializable]
    public enum OnPhase { OnHit, WhenHit, StartTurn, EndTurn, StartRound, EndRound, Permanent }
    
    public enum BattlePhase { Setup, PlayerTurn, NonPlayerTurn, WhosNext, NewRound, Finish, Waiting}
}
