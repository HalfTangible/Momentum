using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace RPG.AbilitySystem.Editor
{
    public class AbilityEditor : EditorWindow
    {
        Ability selectedAbility = null;



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
            //On second thought, should probably make sure we can add a Damage instance to an ability before we worry about the behaviors... still, this should work fine later.
            //    WriteBehaviors(selectedAbility.GetBehaviors());
            EditorGUILayout.LabelField("Ability Name: ");
            selectedAbility.Name = EditorGUILayout.TextField(selectedAbility.Name);
            EditorGUILayout.LabelField("Description: ");
            selectedAbility.Description = EditorGUILayout.TextField(selectedAbility.Description);
            //Display the ability's name
            //Then put up a button that adds a new behavior to the ability based on a drop down menu.
            EditorGUILayout.LabelField("Behaviors:");
        }



        /*
         * private void WriteBehaviors(List<IBehavior> behaviors)
        {
            //GUILayout.BeginArea();

            foreach (IBehavior behavior in behaviors)
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



    }
}