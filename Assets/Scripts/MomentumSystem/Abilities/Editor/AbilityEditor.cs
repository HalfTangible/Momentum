using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

namespace RPG.AbilitySystem.Editor
{
    public class AbilityEditor : EditorWindow
    {
        
        Ability selectedAbility = null;
        
        string newAbilityName = null;
        
        private List<string> behaviorOptions = new List<string>{ "Damage","Healing","Buff" };
        
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
            if(newSelection != null)
            {
                selectedAbility = newSelection;
                newAbilityName = selectedAbility.GetName();
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedAbility == null)
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
            
            
            
            /* I don't actually need to redo the asset mapping/naming and it'd be more trouble than it was worth anyway. Good news is that it worked.
            string tempName = EditorGUILayout.TextField(newAbilityName);

            // Update the new name without renaming immediately
            if (tempName != newAbilityName)
            {
                newAbilityName = tempName;
            }

            // Add a button to confirm the renaming
            if (GUILayout.Button("Rename Ability"))
            {
                if (!string.IsNullOrEmpty(newAbilityName) && newAbilityName != selectedAbility.GetName())
                {
                    RenameAbilityAsset(selectedAbility, newAbilityName);
                    newAbilityName = selectedAbility.GetName();
                    //Repaint();
                    //Tried Repaint to make it update immediately but did not. Will have to settle for it changing after I click off.
                }
            }/*

            /*            EditorGUI.BeginChangeCheck();
                        newAbilityName = EditorGUILayout.TextField(newAbilityName ?? selectedAbility.GetName());
                        if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newAbilityName))
                        {
                            if (newAbilityName != selectedAbility.GetName())
                            {
                                RenameAbilityAsset(selectedAbility, newAbilityName);
                            }
                        }
            */

            EditorGUILayout.LabelField("Description: ");
                
            string newDesc = EditorGUILayout.TextField(selectedAbility.Description);
            if(newDesc != selectedAbility.Description)
            {
                selectedAbility.Description = newDesc;
                EditorUtility.SetDirty(selectedAbility);
                AssetDatabase.SaveAssets();
                Repaint();
            }
            //Then put up a button that adds a new behavior to the ability based on a drop down menu.
            EditorGUILayout.LabelField("Behaviors:");
            ShowBehaviors(selectedAbility);
        }

        private void ShowBehaviors(Ability selectedAbility)
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
                    foreach(string key in behavior.GetKeys())
                    {
                        EditorGUILayout.LabelField($"{key}: ");
                        behavior.SetStat(key, EditorGUILayout.TextField(behavior.GetStat<object>(key).ToString()));
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
                newBehavior = new Damage(10);
                selectedAbility.GetBehaviors().Add(newBehavior);
                EditorUtility.SetDirty(selectedAbility);
                AssetDatabase.SaveAssets();
                Repaint();
            }


        }


        private ABehavior CreateBehavior()
        {
            return null;
        }

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


    }
}