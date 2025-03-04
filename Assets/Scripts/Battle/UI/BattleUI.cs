using UnityEngine;
using UnityEngine.UI; // For UI elements like Text or TMP_Text
using RPG.StatSystem; // Assuming StatSheet is here
using Battlephase;    // For BattlePhase enum

//Thank you Grok
namespace RPG.Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Player UI Elements")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerMomentumText;
        [SerializeField] private Text playerHealthText;

        [Header("NPC UI Elements")]
        [SerializeField] private Text npcNameText;
        [SerializeField] private Text npcMomentumText;
        [SerializeField] private Text npcHealthText;

        [Header("Battle State")]
        [SerializeField] private Text turnIndicatorText;

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
        }

        void Update()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (playerSheet != null)
            {
                playerNameText.text = playerSheet.GetName();
                playerMomentumText.text = $"Momentum: {playerSheet.GetMomentumCurrent()}";
                playerHealthText.text = $"HP: {playerSheet.GetHealthCurrent()}";
            }

            if (npcSheet != null)
            {
                npcNameText.text = npcSheet.GetName();
                npcMomentumText.text = $"Momentum: {npcSheet.GetMomentumCurrent()}";
                npcHealthText.text = $"HP: {npcSheet.GetHealthCurrent()}";
            }

            if (battleEngine != null)
            {
                switch (battleEngine.GetCurrentPhase())
                {
                    case Battlephase.BattlePhase.PlayerTurn:
                        turnIndicatorText.text = "Player's Turn";
                        break;
                    case Battlephase.BattlePhase.NonPlayerTurn:
                        turnIndicatorText.text = "Enemy's Turn";
                        break;
                    default:
                        turnIndicatorText.text = "Waiting...";
                        break;
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