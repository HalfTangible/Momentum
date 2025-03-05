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

        // Start is called before the first frame update
        void Start()
        {

            battleUI = FindObjectOfType<BattleUI>();
            if (battleUI == null) Debug.LogError("BattleUI not found in scene!");

            // Initialize test characters
            player = new StatSheet { characterName = "Hero" };
            npc = new StatSheet { characterName = "Goblin" };

            Setup();
            battleUI?.RefreshUI(player, npc); // Initial UI update
            WhosNext(player, npc);           // Start the battle

        }

        // Update is called once per frame
        void Update()
        {
            battleUI.RefreshUI(player, npc);
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

        public void Setup()
        {
            currentPhase = BattlePhase.Setup;
            Debug.Log("Battle setup complete.");
        }

        public void WhosNext(StatSheet player, StatSheet npc)
        {
            Debug.Log("WhosNext called");
            //This happens first because I just remembered end-of-round effects are a thing I want to do later.
            //If either the player or the NPC are at 0 Momentum, then the round ends and a new one begins that refreshes their momentum.
            if (player.GetMomentumCurrent() == 0 || npc.GetMomentumCurrent() == 0)
            {
                Debug.Log("If1 in");
                NewRound();
            }

            Debug.Log("If1 out");

            //Check the sheets of our two combatants (for now; later we'll make it so it returns the next person in each party).
            //If one of these characters is dead, then that side loses.
            if (player.GetHealthCurrent() <= 0)
            {

                Debug.Log("If2a in");
                PlayerLose();
            }
            else if (npc.GetHealthCurrent() <= 0)
            {

                Debug.Log("If2b in");
                PlayerWin();
            }


            Debug.Log($"If2 out. Also: npc momentum = {npc.GetMomentumCurrent()} and player momentum = {player.GetMomentumCurrent()}");


            //The sheet with the higher Momentum goes next.

            if (npc.GetMomentumCurrent() > player.GetMomentumCurrent())
            {

                Debug.Log("If3A in");
                currentPhase = BattlePhase.NonPlayerTurn; }
            else if (player.GetMomentumCurrent() > npc.GetMomentumCurrent())
            {

                Debug.Log("If3B in");
                currentPhase = BattlePhase.PlayerTurn; 
            }
            else if (currentPhase != BattlePhase.PlayerTurn && currentPhase != BattlePhase.NonPlayerTurn)
            {
                currentPhase = BattlePhase.PlayerTurn; //I'd rather it be random but giving the player an edge is fine and it's also a test RN so who cares
                Debug.Log("If3C in");
            }

            Debug.Log("If3 out");
        }

        public void PlayerTurn(StatSheet player)
        {
            //Player selects an action and spends Momentum.
            // Placeholder: For now, just reduce Momentum and deal damage
            Debug.Log("Player's turn!");
            player.SetMomentumCurrent(player.GetMomentumCurrent() - 3); // "Cost" of action
            npc.TakesDamage(2); // Basic attack
            EndTurn();
        }

        public void NonPlayerTurn(StatSheet npc)
        {
            //Eventually we'll want to assign viable attacks and weights to decide what ability each character uses.
            //For now, just have them select heavy or basic attacks at random.

            Debug.Log("NPC's turn!");
            // Randomly pick "basic" (2 Momentum, 1 damage) or "heavy" (5 Momentum, 3 damage)
            bool useHeavy = Random.value > 0.5f && npc.GetMomentumCurrent() >= 5;
            int momentumCost = useHeavy ? 5 : 2;
            int damage = useHeavy ? 3 : 1;

            npc.SetMomentumCurrent(npc.GetMomentumCurrent() - momentumCost);
            player.TakesDamage(damage);

            EndTurn();
        }

        public void EndTurn()
        {
            currentPhase = BattlePhase.WhosNext;
            battleUI?.RefreshUI(player, npc); // Update UI after turn
        }

        public void NewRound()
        {
            //Give all stat sheets in the current battle a refresh on their momentum.
            //Increase their momentum by their motive
            if (currentPhase == BattlePhase.Setup)
            {
                
            }
            else 
            {
                player.NewRound(); //Player and NPC gain their Momentum for the round, based on Motive.
                npc.NewRound();
            }
            
            Debug.Log("New round started!");
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


    }
}

namespace Battlephase
{
    [System.Serializable]
    public enum OnPhase { OnHit, WhenHit, StartTurn, EndTurn, StartRound, EndRound, Permanent }
    
    public enum BattlePhase { Setup, PlayerTurn, NonPlayerTurn, WhosNext, NewRound, Finish, Waiting}
}
