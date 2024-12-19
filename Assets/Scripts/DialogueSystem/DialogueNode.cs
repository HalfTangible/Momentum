using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        //public string uniqueID;
        //ScriptableObjects have their own name, do not require a uniqueID anymore
        [SerializeField]
        private string speaker; //should make this an enum or a drop-down list later.
        [SerializeField] 
        private string text;
        [SerializeField] 
        private List<string> childNodes = new List<string>();
        [SerializeField]
        private Rect rect = new Rect(0, 0, 200, 200);
        
        public Rect GetRect()
        { 
            return rect; 
        }

        public string GetSpeaker()
        {
            return speaker;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return childNodes;
        }

        public List<string> GetChildNodes()
        {
            return GetChildren();
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetSpeaker(string speaker)
        {
           
            if (this.speaker != speaker)
            {
                Undo.RecordObject(this, "Update Dialogue Speaker"); //Lets you undo a change in dialogue in the edit menu
                this.speaker = speaker;
            EditorUtility.SetDirty(this);
            }
        }
       

        public void SetText(string text)
        {
            
            if (this.text != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text"); //Lets you undo a change in dialogue in the edit menu
                this.text = text;
            EditorUtility.SetDirty(this);
            }
        }

        public void SetChildren(List<string> childNodes)
        {
            this.childNodes = childNodes;
            
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add child"); //Lets you undo a change in dialogue in the edit menu
            childNodes.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove child"); //Lets you undo a change in dialogue in the edit menu
            childNodes.Remove(childID);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
