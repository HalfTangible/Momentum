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
        public event System.Action<BattlePhase> OnPhaseChanged;

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

            List<Ability> abilityTestPool = new List<Ability>();

            abilityTestPool.Add(Resources.Load<Ability>("BasicAttack_Test"));
            abilityTestPool.Add(Resources.Load<Ability>("BasicMultihit_Test")); //Goes off as expected. Have not tried to test its Overwhelm effect yet.
            abilityTestPool.Add(Resources.Load<Ability>("BasicSpell_Test"));

            abilityTestPool.Add(Resources.Load<Ability>("BasicCounter_Test"));
            abilityTestPool.Add(Resources.Load<Ability>("BasicGrit_Test"));
            abilityTestPool.Add(Resources.Load<Ability>("BasicWard_Test"));
            abilityTestPool.Add(Resources.Load<Ability>("BasicShield_Test"));

            abilityTestPool.Add(Resources.Load<Ability>("HeavyAttack_Test"));

            foreach (Ability ability in abilityTestPool)
            {
                Debug.Log($"Adding {ability.name} to the PC and NPC");
                player.AddAbility(Instantiate(ability));
                npc.AddAbility(Instantiate(ability));
            }

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

            if (selectedAbility == null)
            {
                Debug.LogError("Player selected NULL ability!");
                return;
            }

            if (currentPhase != BattlePhase.Waiting) return;

            Debug.Log($"Player selected: {selectedAbility.name}");

            // Execute the chosen ability
            StatSheet attacker = playerParty[0];
            StatSheet defender = enemyParty[Random.Range(0, enemyParty.Count)]; // or target selection later

            ExecuteAbility(attacker, defender, selectedAbility);

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
                ExecuteAbility(attacker, defender, selectedAbility);
            }
            else
            {
                Debug.LogWarning($"Character during {currentPhase} has no abilities (somehow)");
                return;
            }
        }

        private void ExecuteAbility(StatSheet user, StatSheet target, Ability ability)
        {

            if (ability == null)
            {
                Debug.LogError("ExecuteAbility: NULL ability passed!");
                return;
            }

            Debug.Log($"ExecuteAbility called with ability: '{ability.name}'");

            // Fetch behaviors once
            List<ABehavior> behaviors = ability.GetBehaviors();

            // Phase 1: BeforeHit effects
            foreach (ABehavior behavior in behaviors)
            {
                
                if (behavior.actsBeforeHit())
                {
                    Debug.Log($"{behavior.name} triggers in ExecuteAbility (BeforeHit)");
                    StatSheet affected = behavior.hitsUser() ? user : target;
                    behavior.Affects(affected);
                }
            }


            if (ability is Multihit multihit)
            {
                int times = multihit.getRepeats();
                do
                {
                    Debug.Log($"Multihit loops {times} more times.");
                    times--;
                    ExecuteAbility_OnHit(user, target, ability);
                }while (times > 0);
                
            }
            else
            {
                ExecuteAbility_OnHit(user, target, ability);
            }
            // Phase 5: AfterHit effects
            foreach (ABehavior behavior in behaviors)
            {
                
                if (behavior.actsAfterHit())
                {
                    Debug.Log($"{behavior.name} triggers in ExecuteAbility (AfterHit)");
                    StatSheet affected = behavior.hitsUser() ? user : target;
                    behavior.Affects(affected);
                }
            }

            // Phase 6: Cleanup – finish non-continuing behaviors
            foreach (ABehavior behavior in behaviors)
            {
                StatSheet affected = behavior.hitsUser() ? user : target;

                if (!behavior.Continues())
                {
                    Debug.Log($"{behavior.name} triggers in ExecuteAbility (Finished)");
                    behavior.Finished(affected);
                } else if (behavior.Continues())
                {
                    affected.AbilityHit(behavior);
                }
            }

            // Optional: Refresh UI after execution
            battleUI?.RefreshUI(player, npc);
        }

        private void ExecuteAbility_OnHit(StatSheet user, StatSheet target, Ability ability)
        {

            if (ability == null)
            {
                Debug.LogError("ExecuteAbility_OnHit called with NULL ability!");
                return;
            }
            else 
            {
                Debug.Log($"{ability.name} triggers in BattleEngine.ExecuteAbility_OnHit");
            }




            List<ABehavior> behaviors = ability.GetBehaviors();

            // Phase 2: Determine Overwhelming
            
            bool overwhelming = BattleUtility.CheckOverwhelm(user, target);
            Debug.Log($"BattleEngine: Overwhelming? {overwhelming}");

            if (overwhelming)
            {
                Debug.Log($"Overwhelm triggered for {ability.name} used by {user.characterName} on {target.characterName}!");
            }

            // Phase 3: OnHit effects
            foreach (ABehavior behavior in behaviors)
            {
                Debug.Log($"{behavior.name} triggers in ExecuteAbility_OnHit");
                if (behavior.actsOnHit())
                {
                    StatSheet affected = behavior.hitsUser() ? user : target;
                    behavior.Affects(affected);

                    if (overwhelming && !behavior.hitsUser())
                    {
                        behavior.Overwhelms(target);
                    }
                }
            }
            Debug.Log($"{ability.name} pays {ability.MomentumCost} momentum.");
            // Phase 4: Pay momentum (always from user)
            user.SpendMomentum(ability.MomentumCost);
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
