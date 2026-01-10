using UnityEngine;
using UnityEngine.UI; // For UI elements like Text or TMP_Text
using RPG.StatSystem; // Assuming StatSheet is here
using Battlephase;    // For BattlePhase enum
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using RPG.AbilitySystem;

//Thank you Grok
namespace RPG.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Player UI Elements")]
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text playerMomentumText;
        [SerializeField] private TMP_Text playerHealthText;

        [Header("NPC UI Elements")]
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcMomentumText;
        [SerializeField] private TMP_Text npcHealthText;

        [Header("Battle State")]
        [SerializeField] private TMP_Text turnIndicatorText;

        [Header("Ability Selection")]
        [SerializeField] private GameObject abilityButtonPrefab;  // Prefab: Button with TMP_Text child
        [SerializeField] private Transform abilityPanel;          // Panel with VerticalLayoutGroup

        [SerializeField] private BattleEngine battleEngine; // Drag in Inspector



        // Temporary for testing
        [SerializeField] private RPG.StatSystem.StatSheet playerSheet;
        [SerializeField] private RPG.StatSystem.StatSheet npcSheet;

        private List<GameObject> createdAbilityButtons = new List<GameObject>();

        void Start()
        {
            if (battleEngine == null)
            {
                battleEngine = FindObjectOfType<BattleEngine>();
                if (battleEngine == null) Debug.LogError("BattleEngine not found!");
            }


            Debug.Log($"PlayerNameText assigned: {playerNameText != null}");
            Debug.Log($"TurnIndicatorText assigned: {turnIndicatorText != null}");

        }

        void Update()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            //Debug.Log($"Updating UI: Player HP = {playerSheet?.health.Current ?? -1}, NPC HP = {npcSheet?.health.Current ?? -1}");

            if (playerSheet != null)
            {
                if (playerNameText == null) Debug.LogError("playerNameText is null!");
                else playerNameText.text = playerSheet.characterName;

                if (playerMomentumText == null) Debug.LogError("playerMomentumText is null!");
                else playerMomentumText.text = $"Momentum: {playerSheet.momentum.Current}";

                if (playerHealthText == null) Debug.LogError("playerHealthText is null!");
                else playerHealthText.text = $"HP: {playerSheet.health.Current}";
            }

            if (npcSheet != null)
            {
                if (npcNameText == null) Debug.LogError("npcNameText is null!");
                else npcNameText.text = npcSheet.characterName;

                if (npcMomentumText == null) Debug.LogError("npcMomentumText is null!");
                else npcMomentumText.text = $"Momentum: {npcSheet.momentum.Current}";

                if (npcHealthText == null) Debug.LogError("npcHealthText is null!");
                else npcHealthText.text = $"HP: {npcSheet.health.Current}";
            }

            if (battleEngine != null)
            {
                if (turnIndicatorText == null) Debug.LogError("turnIndicatorText is null!");
                else
                {
                    switch (battleEngine.GetCurrentPhase())
                    {
                        case BattlePhase.PlayerTurn:
                            turnIndicatorText.text = "Player's Turn";
                            break;
                        case BattlePhase.NonPlayerTurn:
                            turnIndicatorText.text = "Enemy's Turn";
                            break;
                        case BattlePhase.Finish:
                            turnIndicatorText.text = playerSheet.health.Current <= 0 ? "Defeat!" : "Victory!";
                            break;
                        default:
                            turnIndicatorText.text = "Waiting...";
                            break;
                    }
                }
            }
        }

        public void ShowAbilitySelection(StatSheet player)
        {
            // Clear previous buttons
            ClearAbilityButtons();

            // Create buttons dynamically
            var abilities = player.GetAbilities();

            if (abilities == null || abilities.Count == 0)
            {
                Debug.LogWarning("Player has no abilities!");
                return;
            }

            foreach (var ability in abilities)
            {
                GameObject buttonObj = Instantiate(abilityButtonPrefab, abilityPanel);
                Button button = buttonObj.GetComponent<Button>();
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>(true);
                Debug.Log($"Ability name: {ability.AbilityName}");
                buttonText.text = $"{ability.AbilityName} Cost: {ability.MomentumCost}";  // Customize display

                // Lambda captures the specific ability (no closure issues)
                Ability capturedAbility = ability;
                button.onClick.AddListener(() => battleEngine.PlayerSelectsAbility(capturedAbility));

                createdAbilityButtons.Add(buttonObj);
            }

            if (abilityPanel != null)
            {
                Canvas.ForceUpdateCanvases(); // Helps a lot with text sizing
                LayoutRebuilder.ForceRebuildLayoutImmediate(abilityPanel.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(abilityPanel.GetComponent<RectTransform>()); // Twice is common hack
            }
        }


        public void HideAbilitySelection()
        {
            ClearAbilityButtons();
        }

        private void ClearAbilityButtons()
        {
            foreach (var buttonObj in createdAbilityButtons)
            {
                if (buttonObj != null)
                {
                    Destroy(buttonObj);
                }
            }
            createdAbilityButtons.Clear();
        }

        // Call this from BattleEngine to update specific sheets
        public void RefreshUI(RPG.StatSystem.StatSheet player, RPG.StatSystem.StatSheet npc)
        {
            playerSheet = player;
            npcSheet = npc;
            UpdateUI();
        }
    }
}