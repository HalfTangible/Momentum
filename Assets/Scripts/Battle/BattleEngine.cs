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
using UnityEngine.SceneManagement;


namespace RPG.Battle {
    public class BattleEngine : MonoBehaviour
    {
        public event System.Action<BattlePhase> OnPhaseChanged;   // ADD THIS LINE

        BattlePhase currentPhase;
        BattleUI battleUI;
        StatSheet player;
        StatSheet npc;
        bool firstRound;
        private bool advanceTurn = false; //For manual stopping

        List<Ability> playerAbilities;
        List<Ability> npcAbilities;
        List<StatSheet> playerParty;
        List<StatSheet> enemyParty;

        // Start is called before the first frame update
        void Start()
        {
            advanceTurn = false;
            battleUI = FindObjectOfType<BattleUI>();
            if (battleUI == null) Debug.LogError("BattleUI not found in scene!");

            // Initialize parties

            playerParty = new List<StatSheet>();
            enemyParty = new List<StatSheet>();

            // Initialize test characters
            player = new StatSheet { characterName = "Hero" };
            npc = new StatSheet { characterName = "Goblin" };

            Ability basicAttack = Resources.Load<Ability>("BasicAttack_Test");
            Ability heavyAttack = Resources.Load<Ability>("HeavyAttack_Test");

            player.AddAbility(Instantiate(basicAttack));
            player.AddAbility(Instantiate(heavyAttack));

            npc.AddAbility(Instantiate(basicAttack));
            npc.AddAbility(Instantiate(heavyAttack));

            
            playerParty.Add(player);
            enemyParty.Add(npc);

            Setup();
            battleUI?.RefreshUI(player, npc); // Initial UI update

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
                WhosNext();
            }

            if (advanceTurn && currentPhase != BattlePhase.Finish)
            {
                advanceTurn = false;
                //Debug.Log($"At the switch: {currentPhase}");
                switch (currentPhase)
                {
                    case BattlePhase.PlayerTurn:
                        currentPhase = BattlePhase.Waiting;
                        PlayerTurn();
                        break;
                    case BattlePhase.NonPlayerTurn:
                        currentPhase = BattlePhase.Waiting;
                        NonPlayerTurn();
                        break;
                    case BattlePhase.EndTurn:
                        currentPhase = BattlePhase.Waiting;
                        NewRoundCheck();
                        WhosNext();
                        break;
                    case BattlePhase.NewRound:
                        currentPhase = BattlePhase.Waiting;
                        NewRoundCheck();
                        break;

                }
            }
        }

        public void Setup()
        {
            currentPhase = BattlePhase.Setup;
            advanceTurn = false;

            WhosNext(); // Start the battle
            //Debug.Log("Battle setup complete.");
        }

        public void WhosNext()
        {

            BattleUtility.SortParty(playerParty);
            BattleUtility.SortParty(enemyParty);

            StatSheet nextPlayer = playerParty[0];
            StatSheet nextEnemy = enemyParty[0];

            if (!nextPlayer.isAlive())
                PlayerLose();
            else if (!nextEnemy.isAlive())
                PlayerWin();
            else if (nextPlayer.momentum.Current >= nextEnemy.momentum.Current)
                currentPhase = BattlePhase.PlayerTurn;
            else
                currentPhase = BattlePhase.NonPlayerTurn;


        }


        public void PlayerTurn()
        {
           
            Debug.Log("Player's turn - waiting for input");
            currentPhase = BattlePhase.Waiting;           // Pause flow
            battleUI?.ShowAbilitySelection(player);       // Tell UI: "show buttons!"
            OnPhaseChanged?.Invoke(currentPhase);         // Let anyone listening know
            
        }

        public void PlayerSelectsAbility(Ability selectedAbility)
        {
            if (currentPhase != BattlePhase.Waiting) return;

            Debug.Log($"Player selected: {selectedAbility.name}");

            // Execute the chosen ability
            StatSheet attacker = playerParty[0];
            StatSheet defender = enemyParty[Random.Range(0, enemyParty.Count)]; // or target selection later

            selectedAbility.OnHit(attacker, defender);

            // After action, proceed
            currentPhase = BattlePhase.EndTurn;
            OnPhaseChanged?.Invoke(currentPhase);
            battleUI?.HideAbilitySelection();
        }

        public void NonPlayerTurn()
        {
            //Eventually we'll want to assign viable attacks and weights to decide what ability each character uses.
            //For now, just have them select heavy or basic attacks at random.

            Debug.Log("NPC's turn!");
            
            CharacterTurn(enemyParty, playerParty);

            EndTurn();
            
        }

        public void CharacterTurn(List<StatSheet> party, List<StatSheet> opposingParty)
        {
            //Selects a random attack.

            StatSheet attacker = party[0];

            //Select a random foe in the opposingParty
            int temp = Random.Range(0, opposingParty.Count);
            StatSheet defender = opposingParty[temp];


            if (attacker.GetAbilities().Count > 0)
            {
                temp = Random.Range(0, attacker.GetAbilities().Count);
                Ability selectedAbility = attacker.GetAbilities()[temp];
                selectedAbility.OnHit(attacker, defender);
            }
            else
            {
                Debug.LogWarning($"Character during {currentPhase} has no abilities (somehow)");
                return;
            }
        }



        public void EndTurn()
        {
            
            currentPhase = BattlePhase.EndTurn;
            battleUI?.RefreshUI(player, npc); // Update UI after turn
        }

        public void NewRoundCheck()
        {
            bool roundCheck = false;
            
            //Give all stat sheets in the current battle a refresh on their momentum.
            //Increase their momentum by their motive
            if (currentPhase == BattlePhase.Setup)
            {
                
            }
            else
            {
                //Debug.Log($"Player: Momentum is {player.GetMomentumCurrent()}; motive is {player.GetMotiveCurrent()} ");
                //player.NewRound(); //Player and NPC gain their Momentum for the round, based on Motive.
                //Debug.Log($"NPC: Momentum is {npc.GetMomentumCurrent()}; motive is {npc.GetMotiveCurrent()} ");
                //npc.NewRound();
                //Debug.Log($"Player: Momentum is {player.GetMomentumCurrent()}; NPC is {npc.GetMomentumCurrent()} ");
                while (BattleUtility.RoundRefreshCheck(playerParty) || BattleUtility.RoundRefreshCheck(enemyParty))
                {
                    BattleUtility.RoundRefresh(playerParty);
                    BattleUtility.RoundRefresh(enemyParty);
                    roundCheck = true;
                }


                if (roundCheck)
                {
                    Debug.Log("New round!");
                    BattleUtility.ApplyRoundEffects(enemyParty);
                    BattleUtility.ApplyRoundEffects(playerParty);

                }

            }

            
            
            //Debug.Log("New round started!");
            currentPhase = BattlePhase.EndTurn;
            battleUI?.RefreshUI(player, npc);
        }

        public void PlayerWin()
        {
            Debug.Log("Player wins!");
            currentPhase = BattlePhase.Finish;
            Debug.Log($"Returning to: {BattleData.PreviousScene}");
            BattleData.IsReturningFromBattle = true;
            SceneManager.LoadScene(BattleData.PreviousScene);
        }

        public void PlayerLose()
        {
            Debug.Log("Player loses!");
            currentPhase = BattlePhase.Finish;
            Debug.Log($"Returning to: {BattleData.PreviousScene}");
            BattleData.IsReturningFromBattle = true;
            SceneManager.LoadScene(BattleData.PreviousScene);
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
    
    public enum BattlePhase { Setup, PlayerTurn, NonPlayerTurn, EndTurn, NewRound, Finish, Waiting}
}
