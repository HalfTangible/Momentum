using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RuleTileBatchAssign
{
    [MenuItem("Tools/Assign Sliced Sprites to Rule Tile")]
    static void AssignSlicedSprites()
    {
        // Get the selected Rule Tile
        RuleTile ruleTile = Selection.activeObject as RuleTile;
        if (ruleTile == null)
        {
            Debug.LogError("Please select a Rule Tile asset in the Project window.");
            return;
        }

        // Prompt user to select the sprite sheet
        string spriteSheetPath = EditorUtility.OpenFilePanel("Select Sprite Sheet", "Assets", "png");
        if (string.IsNullOrEmpty(spriteSheetPath))
        {
            Debug.LogWarning("No sprite sheet selected.");
            return;
        }

        // Convert to relative path
        spriteSheetPath = "Assets" + spriteSheetPath.Substring(Application.dataPath.Length);

        // Load all sprites from the sprite sheet
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath)
            .OfType<Sprite>()
            .OrderBy(s => ExtractNumberFromName(s.name)) // Sort by number in name
            .ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites found in the selected sprite sheet. Ensure it is sliced properly.");
            return;
        }

        // Assign the first sprite as the default sprite
        ruleTile.m_DefaultSprite = sprites[0];

        // Clear existing tiling rules to avoid conflicts
        ruleTile.m_TilingRules.Clear();

        // Create a tiling rule for each sprite
        foreach (Sprite sprite in sprites)
        {
            RuleTile.TilingRule rule = new RuleTile.TilingRule
            {
                m_Sprites = new Sprite[] { sprite }, // Assign sprite to rule
                m_Neighbors = new List<int>(new int[8]), // Empty neighbor rules
                m_RuleTransform = RuleTile.TilingRule.Transform.Fixed,
                m_Output = RuleTile.TilingRuleOutput.OutputSprite.Single
            };
            ruleTile.m_TilingRules.Add(rule);
        }

        // Force refresh of the Rule Tile
        EditorUtility.SetDirty(ruleTile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); // Ensure Inspector updates
        Debug.Log($"Assigned {sprites.Length} sprites to Rule Tile: {ruleTile.name} in numerical order.");
    }

    // Helper method to extract number from sprite name (e.g., fileName_5 -> 5)
    static int ExtractNumberFromName(string name)
    {
        // Use regex to find the number after the underscore
        Match match = Regex.Match(name, @"_(\d+)$");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int number))
        {
            return number;
        }
        return int.MaxValue; // Fallback for names without a number
    }
}