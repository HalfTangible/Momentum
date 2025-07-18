using UnityEngine;
using UnityEngine.UI; // For UI elements like Text or TMP_Text
using RPG.StatSystem; // Assuming StatSheet is here
using Battlephase;    // For BattlePhase enum
using TMPro;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

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

        [SerializeField] private BattleEngine battleEngine; // Drag in Inspector

        // Temporary for testing
        [SerializeField] private RPG.StatSystem.StatSheet playerSheet;
        [SerializeField] private RPG.StatSystem.StatSheet npcSheet;

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

        // Call this from BattleEngine to update specific sheets
        public void RefreshUI(RPG.StatSystem.StatSheet player, RPG.StatSystem.StatSheet npc)
        {
            playerSheet = player;
            npcSheet = npc;
            UpdateUI();
        }
    }
}