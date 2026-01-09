using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{

        // Array of possible enemy types or prefabs. You can assign these in the Inspector.
        // For simplicity, I'm using strings to represent enemy types (e.g., "Goblin", "Wolf").
        // In your BattleScene, you can use this string to spawn the appropriate enemy.
        public string[] possibleEnemies = { "Goblin", "Wolf", "Slime" };

        // Tag for the player object
        private const string PlayerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
        Debug.Log("Trigger entered by: " + other.name);  // Debug log to confirm trigger fires

        if (other.CompareTag(PlayerTag))
            {
            Debug.Log("Player entered trigger!");  // Debug log for player specifically

            // Decide on an enemy randomly
            string chosenEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Length)];

                // === CRITICAL: Save where we came from ===
                BattleData.PreviousScene = SceneManager.GetActiveScene().name;
                // Store the chosen enemy type in a static variable for access in BattleScene
                BattleData.CurrentEnemy = chosenEnemy;

                // Load the BattleScene
                SceneManager.LoadScene("BattleScene");
            }
        }
    
}

public static class BattleData
{
    public static string CurrentEnemy { get; set; }
    public static string PreviousScene { get; set; }
    public static Vector3 PlayerPosition { get; set; }
    public static Quaternion PlayerRotation { get; set; }
    public static bool IsReturningFromBattle { get; set; }

    public static void Reset()
    {
        CurrentEnemy = null;
        PreviousScene = null;
        PlayerPosition = Vector3.zero;
        PlayerRotation = Quaternion.identity;
        IsReturningFromBattle = false;
    }
}