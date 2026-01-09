using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleReturnHandler : MonoBehaviour
{
    private void Awake()
    {
        // Runs early on scene load. Restores player position if returning from battle.
        if (BattleData.IsReturningFromBattle)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Debug.Log($"Restoring player to pos: {BattleData.PlayerPosition}, rot: {BattleData.PlayerRotation.eulerAngles}");
                player.transform.SetPositionAndRotation(BattleData.PlayerPosition, BattleData.PlayerRotation);
            }
            else
            {
                Debug.LogError("Player not found on battle return!");
            }

            BattleData.Reset();  // Clear data to prevent issues
            Debug.Log("Battle data reset.");
        }
    }
}