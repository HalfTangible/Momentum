using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.Linq;
using System;

namespace RPG.AbilitySystem.Editor
{
    public class AbilityEditor : EditorWindow
    {

        Ability selectedAbility = null;

        string newAbilityName = null;

        private List<string> behaviorOptions = new List<string> { "Damage", "Healing", "Buff", "Debuff" };

        private int selectedBehaviorIndex = 0;



        [MenuItem("Window/Ability Editor")]
        public static void ShowEditorWindow()
        {
            //UnityEngine.Debug.Log("ShowEditorWindow called");
            GetWindow(typeof(AbilityEditor), false, "Ability Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {

            //UnityEngine.Debug.Log("OpenDialogue function called");

            Ability ability = EditorUtility.InstanceIDToObject(instanceId) as Ability;

            if (ability != null)
            {
                //UnityEngine.Debug.Log("OpenDialogue: true");

                AbilityEditor editor = (AbilityEditor)GetWindow(typeof(AbilityEditor), false, "Ability Editor");
                editor.selectedAbility = ability;
                editor.Repaint(); // Ensures the editor updates immediately

                //ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged; //DO NOT add the brackets, we're adding it to a list to be called later on
        }

        private void OnSelectionChanged()
        {
            Ability newSelection = Selection.activeObject as Ability;
            if (newSelection != null)
            {
                selectedAbility = newSelection;
                newAbilityName = selectedAbility.GetName();
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedAbility == null)
            {
                EditorGUILayout.LabelField("No ability selected");
            }
            else
            {
                DisplayAbility();

            }
        }

        private void DisplayAbility()
        {
            if (selectedAbility == null) return;

            if (newAbilityName == null)
            {
                newAbilityName = selectedAbility.GetAbilityName();
            }
            //On second thought, should probably make sure we can add a Damage instance to an ability before we worry about the behaviors... still, this should work fine later.
            //    WriteBehaviors(selectedAbility.GetBehaviors());
            EditorGUILayout.LabelField("Ability Name: ");
            newAbilityName = EditorGUILayout.TextField(selectedAbility.GetAbilityName());

            if (newAbilityName != selectedAbility.GetAbilityName())
            {
                selectedAbility.SetAbilityName(newAbilityName);
                EditorUtility.SetDirty(selectedAbility);
                AssetDatabase.SaveAssets();
                Repaint();
            }
            //RenameAbilityAsset((EditorGUILayout.TextField(selectedAbility.GetName())));


            EditorGUILayout.LabelField("Description: ");

            string newDesc = EditorGUILayout.TextField(selectedAbility.Description);
            if (newDesc != selectedAbility.Description)
            {
                selectedAbility.Description = newDesc;
                EditorUtility.SetDirty(selectedAbility);
                AssetDatabase.SaveAssets();
                Repaint();
            }
            //Then put up a button that adds a new behavior to the ability based on a drop down menu.
            EditorGUILayout.LabelField("Behaviors:");
            ShowBehaviors();
        }

        private void ShowBehaviors()
        {
            
            if (selectedAbility.GetBehaviors() != null)
            {

                GUILayout.BeginHorizontal();
                selectedBehaviorIndex = EditorGUILayout.Popup("Select behavior to add:", selectedBehaviorIndex, behaviorOptions.ToArray());
                if (GUILayout.Button("Add Behavior"))
                {
                    CreateNewBehavior();
                }
                GUILayout.EndHorizontal();
            }

            List<ABehavior> behaviorsToDelete = new List<ABehavior>();
            //This is to keep the InvalidOperationException from popping up; it was doing so because we were moving something in a list while iterating over it.
            //I feel like it would've been fine but just to be safe...

            foreach (var behavior in selectedAbility.GetBehaviors())
            {
                EditorGUILayout.LabelField($"{behavior.GetType().Name} stats:");

                foreach (string key in behavior.GetAllKeys())
                {


                    object statValue = behavior.GetStat<object>(key);
                    object newValue = statValue; // Default to the same value if no changes are made
                    //UnityEngine.Debug.Log($"Top: statValue = {statValue}, newValue = {newValue}");
                    //UnityEngine.Debug.Log($"Top Comparison between {statValue} and {newValue}: {statValue.Equals(newValue)}");
                    // Retrieve the current stat value associated with the key
                    //object currentStatValue = behavior.GetStat<object>(key);
                    //string currentValueString = currentStatValue?.ToString() ?? string.Empty;

                    switch (statValue)
                    {
                        case int intValue:
                            newValue = EditorGUILayout.IntField(key, intValue);
                            break;

                        case float floatValue:
                            newValue = EditorGUILayout.FloatField(key, floatValue);
                            break;

                        case bool boolValue:
                            newValue = EditorGUILayout.Toggle(key, boolValue);
                            break;

                        case string stringValue:
                            newValue = EditorGUILayout.TextField(key, stringValue);
                            break;

                        case Enum enumValue:
                            newValue = EditorGUILayout.EnumPopup(key, enumValue);
                            break;

                        case IList listValue:

                            // Convert the list into an array of string options
                            string[] options = listValue.Cast<object>().Select(o => o.ToString()).ToArray();

                            // Retrieve the current stat value associated with the key
                            object currentStatValue = behavior.GetStat<object>(key);
                            string currentValueString = currentStatValue?.ToString() ?? string.Empty;

                            // Find the current index of the stat value in the options array
                            int currentIndex = Array.IndexOf(options, currentValueString);

                            // Ensure a valid default selection index
                            //currentIndex = Mathf.Max(0, currentIndex);
                            currentIndex = behavior.GetListIndex(key);
                            // Display the dropdown menu and get the new selected index
                            int newIndex = EditorGUILayout.Popup(key, currentIndex, options);

                            //UnityEngine.Debug.Log($"newIndex = {newIndex}, currentIndex = {currentIndex}");

                            //So the problem is that the newValue assigned at the end of this is a string to represent the selected value
                            //But since the old value is a List, they will always be different, and thus the test always fails
                            //We need to compare the newIndex with the currentIndex. Which we now have saved to the behavior.
                            //If the newIndex is changed, then we change the newValue, and the update occurs as it should.

                            if(newIndex != currentIndex)
                            {
                                //If this has happened, then you need to set the newValue to the newIndex.
                                newValue = newIndex;
                            }

                            

                            break;
                    }

                    if (!Equals(statValue, newValue))
                    {
                        //UnityEngine.Debug.Log($"Bot: statValue = {statValue}, newValue = {newValue}");
                        //UnityEngine.Debug.Log($"Bot Comparison between {statValue} and {newValue}: {statValue.Equals(newValue)}");
                        //UnityEngine.Debug.Log("Values different, saving now");
                        behavior.SetStat(key, newValue);
                        EditorUtility.SetDirty(behavior);
                        AssetDatabase.SaveAssets();
                    }

                }

                if (GUILayout.Button("Delete behavior"))
                {
                    behaviorsToDelete.Add(behavior);    
                    
                }


            }

            foreach (var behavior in behaviorsToDelete)
            {
                selectedAbility.GetBehaviors().Remove(behavior);
                AssetDatabase.RemoveObjectFromAsset(behavior);
                EditorUtility.SetDirty(behavior);
                AssetDatabase.SaveAssets();
            }


        }
        private void CreateNewBehavior()
        {
            ABehavior newBehavior = null;

            string newBehaviorName = behaviorOptions[selectedBehaviorIndex];


            switch (newBehaviorName)
            {
                case "Damage":
                    newBehavior = ScriptableObject.CreateInstance<Damage>();

                    break;
                case "Healing":
                    newBehavior = ScriptableObject.CreateInstance<Healing>();

                    break;
                case "Buff":
                    newBehavior = ScriptableObject.CreateInstance<Buff>();
                    break;
            }

            newBehavior.Initialize(10);
            AssetDatabase.AddObjectToAsset(newBehavior, selectedAbility);
            selectedAbility.GetBehaviors().Add(newBehavior);
            EditorUtility.SetDirty(selectedAbility);
            EditorUtility.SetDirty(newBehavior);
            AssetDatabase.SaveAssets();
            Repaint();
        }
        //throw new NotImplementedException("You didn't finish the creation of a new behavior method.");
    }
}

        /*        private void ShowBehaviors()
                {
                    //Each behavior should get its own node.
                    //There should be an option at the end of the list to add more behaviors
                    //Connected to that should be a list of the valid behavior types. For now it's just damage.
                    //If a behavior is added, we need to Repaint.
                    if (selectedAbility.GetBehaviors() != null)
                    {
                        foreach (var behavior in selectedAbility.GetBehaviors())
                        {
                            //GUILayout.BeginVertical();
                            EditorGUILayout.LabelField($"{behavior.GetType().Name} stats:");
                            //GUILayout.BeginHorizontal();
                            //Show the different behavior stats
                            foreach(string key in behavior.GetAllKeys())
                            {

                                EditorGUILayout.LabelField($"{key}: ");
                                string previousString = behavior.GetStat<object>(key).ToString();
                                string currentString = EditorGUILayout.TextField(previousString);

                                if(previousString != currentString)
                                {
                                    behavior.SetStat(key, currentString);
                                    EditorUtility.SetDirty(behavior);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                            //Add button to remove each behavior
                            //GUILayout.EndHorizontal();
                            //GUILayout.EndVertical();
                        }
                    }

                    if (GUILayout.Button("Add Behavior"))
                    {
                        // Example of creating a new Damage behavior (or any other behavior you want)
                        ABehavior newBehavior;

                        //If damage is selected
                        /*newBehavior = new Damage(10);*/

        /*
        newBehavior = ScriptableObject.CreateInstance<Damage>();
                newBehavior.Initialize(10);
                AssetDatabase.AddObjectToAsset(newBehavior, selectedAbility);
                selectedAbility.GetBehaviors().Add(newBehavior);
                EditorUtility.SetDirty(selectedAbility);
                EditorUtility.SetDirty(newBehavior);
                AssetDatabase.SaveAssets();
                Repaint();
            }


        }*/

        /*
         * private void WriteBehaviors(List<ABehavior> behaviors)
        {
            //GUILayout.BeginArea();

            foreach (ABehavior behavior in behaviors)
            {
                GUILayout.BeginHorizontal();
                //Display behavior's name and stats.
                foreach (string key in behavior.GetKeys())
                {
                    var value = behavior.GetStat<object>(key);
                    if (value != null)
                    {
                        //Set up a label for each behavior
                        //Then an editable field
                        EditorGUILayout.LabelField($"{key}:");
                        behavior.SetStat(key, EditorGUILayout.TextField(value.ToString));
                    //Add a delete button
                    }
                }
                GUILayout.EndHorizontal();
            }

            //GUILayout.EndArea();
        }*/
        /*
        private void RenameAbilityAsset(ScriptableObject selectedAbility, string newName)
        {
            RenameAbilityAsset(selectedAbility, newName, 0);
        }

        private void RenameAbilityAsset(ScriptableObject ability, string newName, int count)
        {
            if (ability == null || string.IsNullOrEmpty(newName) || ability.name == newName)
                return;

            // Get the asset path
            string assetPath = AssetDatabase.GetAssetPath(ability);

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Rename the asset file

                string finalName = count > 0 ? $"{newName}_{count}" : newName;

                string directory = System.IO.Path.GetDirectoryName(assetPath);
                string newPath = $"{directory}/{finalName}.asset";


                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(newPath) == null)
                {
                    AssetDatabase.RenameAsset(assetPath, finalName);

                    // Rename the object's name
                    ability.name = finalName;

                    // Save the changes
                    EditorUtility.SetDirty(ability);
                    AssetDatabase.SaveAssets();
                }
                else
                {

                    RenameAbilityAsset(ability, newName, ++count);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to rename: Unable to find the asset path.");
            }
        }

        */


