using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> validSpeakers = new List<string>();
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode> ();

#if UNITY_EDITOR
//This if only occurs when running in the editor, is not even compiled in the game



#endif

        private void OnValidate() //Called when the scriptable object is loaded or when a value is changed in the inspector
        {
            
            //UnityEngine.Debug.Log("OnValidate called.");
            nodeLookup.Clear ();

            //UnityEngine.Debug.Log("Dictionary cleared");
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }

            InitializeListCheck();

        }

        public IEnumerable<DialogueNode> GetAllNodes() //using IEnumerable so the type can be changed later (important thing is that a for loop can be done over it)
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }


        }

        public DialogueNode GetNodeByID(string id)
        {
            IEnumerable<DialogueNode> theList = GetAllNodes();

            DialogueNode theNode = null;

            foreach(DialogueNode node in theList)
            {
                if(node.name == id)
                {
                    theNode= node;
                    break;
                }
            }

            return theNode;
        }

        public void AddSpeaker(string speakerName)
        {
            if (!validSpeakers.Contains(speakerName))
            {
                validSpeakers.Add(speakerName);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        public void RemoveSpeaker(string speakerName)
        {
            if (validSpeakers.Contains(speakerName))
            {
                validSpeakers.Remove(speakerName);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        public void InitializeListCheck()
        {
            if (validSpeakers == null)
            {
                validSpeakers = new List<string>();
            }
            if (!validSpeakers.Contains("Someone"))
            {
                validSpeakers.Add("Someone");
            }
        }

        public List<string> GetSpeakers()
        {
            InitializeListCheck();
            return validSpeakers;
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);
            newNode.SetSpeaker("Someone");

            Undo.RegisterCreatedObjectUndo(newNode, "Created dialogue node");
            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newNode);
           
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private static DialogueNode MakeNode(DialogueNode parentNode)
        {
            //UnityEngine.Debug.Log("CreateNode called");
            DialogueNode newNode = CreateInstance<DialogueNode>();

            newNode.name = System.Guid.NewGuid().ToString();


            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
            }

            return newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);

        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
            //These are the contents of CreateNode, but we don't want to run the Undos because it leads to an infinite loop
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() //When you load a file from a hard drive.
        {
        //Needed because of the ISerializationCallbackReceiver interface. Which you need for OnBeforeSerialize
        }
    }
}