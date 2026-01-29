using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System;
using Debug = UnityEngine.Debug;

namespace RPG.AbilitySystem.Editor
{
    public class AbilityEditor : EditorWindow
    {

        Ability selectedAbility = null;
        string newAbilityName = null;
        //private List<string> behaviorOptions = new List<string> { "Damage", "Healing", "Buff", "Debuff" };
        private int selectedBehaviorIndex = 0;
        private List<Type> behaviorTypes = new List<Type>();
        private List<string> behaviorDisplayNames = new List<string>();

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
            Selection.selectionChanged += OnSelectionChanged;

            behaviorTypes.Clear();
            behaviorDisplayNames.Clear();

            // Discover all concrete ABehavior subclasses once
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ABehavior)) && !type.IsAbstract && !type.IsGenericTypeDefinition)
                    {
                        behaviorTypes.Add(type);

                        string displayName;

                        try
                        {
                            var attr = Attribute.GetCustomAttribute(type, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                            displayName = attr != null ? attr.Name : type.Name.Replace("Behavior", "");
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to get display name for {type.FullName}: {e.Message}");
                            displayName = type.Name; // Fallback
                        }

                        behaviorDisplayNames.Add(displayName);
                    }
                }
            }

            // Safety: ensure lengths match (should never happen, but log if it does)
            if (behaviorTypes.Count != behaviorDisplayNames.Count)
            {
                Debug.LogError($"Mismatch! Types: {behaviorTypes.Count}, Names: {behaviorDisplayNames.Count}");
                // Truncate to shortest
                int min = Mathf.Min(behaviorTypes.Count, behaviorDisplayNames.Count);
                behaviorTypes = behaviorTypes.GetRange(0, min);
                behaviorDisplayNames = behaviorDisplayNames.GetRange(0, min);
            }

            // Sort safely
            var sortedPairs = new List<(string Name, Type Type)>();
            for (int i = 0; i < behaviorDisplayNames.Count; i++)
            {
                sortedPairs.Add((behaviorDisplayNames[i], behaviorTypes[i]));
            }

            sortedPairs.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

            behaviorDisplayNames.Clear();
            behaviorTypes.Clear();

            foreach (var pair in sortedPairs)
            {
                behaviorDisplayNames.Add(pair.Name);
                behaviorTypes.Add(pair.Type);
            }

            selectedBehaviorIndex = Mathf.Clamp(selectedBehaviorIndex, 0, behaviorDisplayNames.Count - 1);
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

            EditorGUILayout.LabelField("Base Ability Properties", EditorStyles.boldLabel);

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

            EditorGUILayout.LabelField("Momentum cost: ");

            int newMomentumCost = EditorGUILayout.IntField(selectedAbility.MomentumCost);
            if (newMomentumCost != selectedAbility.MomentumCost)
            {
                selectedAbility.MomentumCost = newMomentumCost;
                EditorUtility.SetDirty(selectedAbility);
                AssetDatabase.SaveAssets();
                Repaint();
            }

            selectedAbility.Category = (AbilityCategory)EditorGUILayout.EnumPopup("Category", selectedAbility.Category);
            selectedAbility.Type = (AbilityType)EditorGUILayout.EnumPopup("Type", selectedAbility.Type);

            // Draw subclass-specific fields (if any)
            Type abilityType = selectedAbility.GetType();
            if (abilityType != typeof(Ability))
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"Specific Properties ({abilityType.Name})", EditorStyles.boldLabel);

                // Draw all public serialized fields from the subclass
                DrawSerializedFields(selectedAbility, abilityType);
            }


            //Then put up a button that adds a new behavior to the ability based on a drop down menu.
            EditorGUILayout.LabelField("Behaviors", EditorStyles.boldLabel);
            ShowBehaviors();
        }

        private void ShowBehaviors()
        {
            
            if (selectedAbility.GetBehaviors() != null)
            {

                GUILayout.BeginHorizontal();
                selectedBehaviorIndex = EditorGUILayout.Popup("Select behavior to add:", selectedBehaviorIndex, behaviorDisplayNames.ToArray());
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
            if (selectedBehaviorIndex < 0 || selectedBehaviorIndex >= behaviorTypes.Count)
                return;

            Type selectedType = behaviorTypes[selectedBehaviorIndex];

            // Create instance of the selected type
            ABehavior newBehavior = (ABehavior)ScriptableObject.CreateInstance(selectedType);

            newBehavior.Initialize(10);  // Your default init

            AssetDatabase.AddObjectToAsset(newBehavior, selectedAbility);
            selectedAbility.GetBehaviors().Add(newBehavior);

            EditorUtility.SetDirty(selectedAbility);
            EditorUtility.SetDirty(newBehavior);
            AssetDatabase.SaveAssets();
            Repaint();
        }

        // Helper: Draw public serialized fields of a type
        private void DrawSerializedFields(object target, Type type)
        {
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                             .Where(f => f.GetCustomAttribute<SerializeField>() != null);

            foreach (var field in fields)
            {
                object value = field.GetValue(target);
                object newValue = value;

                string label = ObjectNames.NicifyVariableName(field.Name);

                if (field.FieldType == typeof(int))
                    newValue = EditorGUILayout.IntField(label, (int)value);
                else if (field.FieldType == typeof(float))
                    newValue = EditorGUILayout.FloatField(label, (float)value);
                else if (field.FieldType == typeof(bool))
                    newValue = EditorGUILayout.Toggle(label, (bool)value);
                else if (field.FieldType == typeof(string))
                    newValue = EditorGUILayout.TextField(label, (string)value);
                else if (field.FieldType.IsEnum)
                    newValue = EditorGUILayout.EnumPopup(label, (Enum)value);
                else
                {
                    // Fallback: show as object field (for complex types)
                    newValue = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, field.FieldType, true);
                }

                if (!Equals(value, newValue))
                {
                    field.SetValue(target, newValue);
                    MarkDirty(target as UnityEngine.Object);
                }
            }
        }

        private void MarkDirty(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
        //throw new NotImplementedException("You didn't finish the creation of a new behavior method.");
    }
}