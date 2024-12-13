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

    }
}