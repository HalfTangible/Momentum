using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Collections.Specialized;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {

        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized] //does not remember when the editor closes
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 250;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            //UnityEngine.Debug.Log("ShowEditorWindow called");
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {

            //UnityEngine.Debug.Log("OpenDialogue function called");
            
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
            if (dialogue != null )
            {
                //UnityEngine.Debug.Log("OpenDialogue: true");
                DialogueEditor editor = (DialogueEditor)GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
                editor.selectedDialogue = dialogue;
                editor.Repaint(); // Ensures the editor updates immediately
                /*ShowEditorWindow();*/


                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            
            Selection.selectionChanged += OnSelectionChanged; //DO NOT add the brackets, we're adding it to a list to be called later on

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            Dialogue newSelection = Selection.activeObject as Dialogue;
            if (newSelection != null)
            {
                selectedDialogue = newSelection;
                Repaint();
            }

            //UnityEngine.Debug.Log("Selection changed");
        }

        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                //EditorGUILayout.LabelField(selectedDialogue.name);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize); //Reserves space on the canvas for the nodes since they're not auto laid out
                //Texture goes here so that it's behind everything we're drawing
                Texture2D backgroundTexture = Resources.Load("DialogueBackground") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, texCoords);


                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    //DrawNode(node);
                    DrawConnections(node);
                }

                //Putting these in two separate foreach loops causes the lines to be drawn beneath the nodes.

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                    //DrawConnections(node);
                }

                EditorGUILayout.EndScrollView();

                //C# does not like it when you add something to a list while iterating over it.
                //Therefore we're going to add the node separately.

                if(creatingNode != null)
                {
                    
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if(deletingNode != null)
                {
                    
                    selectedDialogue.DeleteNode(deletingNode); 
                    deletingNode = null;
                }
            }
            //Repaint(); //Redraws the UI (but does it on every frame so not a great idea unless you gotta)
            //EditorGUI.LabelField(new Rect(10, 10, 200, 200), "Hello World");
            //EditorGUILayout.LabelField("Hello World");
            

        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                //if null, the rest of the if statement is ignored.
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                } else if (draggingNode == null)
                {
                    //Record offset and dragging
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                //Repaint(); //option
                GUI.changed = true; //Signals whether any data has been changed, calls OnGUI again
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }




        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.GetRect(), nodeStyle); //Begins the node layout
            EditorGUILayout.LabelField("Node:"); //Can also add editor styles as a second input variable, such as EditorStyles.whiteLabel
            
            //EditorGUILayout.LabelField("Unique ID:");
            //string newId = EditorGUILayout.TextField(node.name);
            //GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speaker:");
            node.SetSpeaker(EditorGUILayout.TextField(node.GetSpeaker()));
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text:");
            node.SetText(EditorGUILayout.TextField(node.GetText()));
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("+"))
            {
                //UnityEngine.Debug.Log("+ button pressed!");
                creatingNode = node;
                //UnityEngine.Debug.Log("Node created");
                //selectedDialogue.CreateNode(node);
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("-"))
            {
                deletingNode = node;
            }

            GUILayout.EndHorizontal();


            GUILayout.EndArea(); //Ends the node layout
        }

        /* // Unneeded; List<> has the Contains function.
        private bool isAChildNode(string childID, DialogueNode parentNode)
        {
            foreach (string id in parentNode.childNodes) {
                if (id == childID)
                    return true;
            }
            return false;
        }*/

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y); //xMax is the right

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {    
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y); //xMin is the left
                Vector3 controlPointOffset = new Vector2(100, 0);
                //Vector3 controlPointOffset = startPosition - endPosition;
                //controlPointOffset.y = 0;
                //controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            //You do a foundNode and return that so that you'll select the node on top, rather than the first one in the list.
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(mousePosition))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else
            {

                if (node == linkingParentNode)
                {
                    if (GUILayout.Button("Cancel"))
                    {
                        Undo.RecordObject(selectedDialogue, "Cancel selection of parent node");
                        linkingParentNode = null;
                    }
                }
                else if (linkingParentNode.GetChildren().Contains(node.name))
                {
                    if (GUILayout.Button("Unlink"))
                    {
                        Undo.RecordObject(selectedDialogue, "Remove dialogue link");
                        linkingParentNode.RemoveChild(node.name);
                        linkingParentNode = null;
                    }
                }
                else 
                {
                    if (GUILayout.Button("Child"))
                    {
                        Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                        linkingParentNode.AddChild(node.name);
                        linkingParentNode = null;
                    }
                }
            }
        }

            

        private void OnClick()
        {
            
        }
    }
}