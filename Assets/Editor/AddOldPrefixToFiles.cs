using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class AddOldPrefixToFiles : EditorWindow
{
    
    private string targetFolder = "Assets/Old/OldAbilities"; // Adjust to your folder location
    private string filePattern = "*.cs"; // Adjust if needed to target specific files (e.g., "*.Behavior.cs")

    [MenuItem("Tools/Add 'Old' Prefix to Files")]
    public static void ShowWindow()
    {
        GetWindow<AddOldPrefixToFiles>("Add 'Old' Prefix to Files");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add 'Old' Prefix to Files", EditorStyles.boldLabel);

        targetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);
        filePattern = EditorGUILayout.TextField("File Pattern", filePattern);

        if (GUILayout.Button("Add Prefix"))
        {
            AddPrefixToFiles();
        }
    }

    private void AddPrefixToFiles()
    {
        string[] files = Directory.GetFiles(targetFolder, filePattern, SearchOption.AllDirectories);

        foreach (string filePath in files)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (!fileName.StartsWith("Old") && fileName.StartsWith("Ability"))
            {
                string newFilePath = Path.Combine(directory, "Old" + fileName);
                File.Move(filePath, newFilePath);
                UnityEngine.Debug.Log($"Renamed: {fileName} to Old{fileName}");
            }
        }

        AssetDatabase.Refresh();
    }
}