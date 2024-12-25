using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Dialogue;
using Debug = UnityEngine.Debug;
using TMPro;

namespace TextHandler.RPG {
    
    public class DisplayDialogue : MonoBehaviour
    {
        private Dialogue theDialogue;
        private DialogueNode currentNode;
        private DialogueNode nextNode;

        public GameObject dialogueBox;
        
        float textSpeed;
        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI dialogueText;

        void Start()
        {
            DialogueTest();
            StartDialogue();
            textSpeed = 2f;
        }

        private void DialogueTest()
        {
            theDialogue = Resources.Load<Dialogue>("TestDialogue");

            if (theDialogue == null)
            {
                Debug.LogError("TestDialogue asset not found in Resources folder!");
            }
            else
            {
                Debug.Log("Dialogue loaded successfully");
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Check if the Menu object is active. If it is, then we need to pause all coroutines and ignore all input.
            //Probably should do that with a public utility class rather than try to do it in every individual class
            //Let's wait on making the text work like this and focus on getting it to display.
            
            /*
            
            if (Input.GetMouseButtonDown(0))//Left mouse button
            {
                if (textComponent.text == lines[index])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }*/
        }

        void StartDialogue()
        {
            Debug.Log("Accessing the root node.");
            currentNode = theDialogue.GetRootNode();
            Debug.Log($"Root node accessed. Text is {currentNode.GetText()}");
            StartCoroutine(DisplayCurrentNode());
            Debug.Log("Coroutine begun.");
        }

        /*
        IEnumerator TypeLine()
        {
            foreach (char c in lines[index].ToCharArray())
            {
                textComponent.text += c;
                yield return new WaitForSeconds(textSpeed);
            }
        }*/

        IEnumerator DisplayCurrentNode()
        {
            yield return new WaitForSeconds(textSpeed);

            while (currentNode != null)
                {
                    speakerText.text = currentNode.GetSpeaker();
                    dialogueText.text = currentNode.GetText(); //Eventually should be typed across the screen

                    yield return new WaitForSeconds(textSpeed);

                    currentNode = GetNextNode();
                }
            
        }




        DialogueNode GetNextNode()
        {
            List<string> childIDs = currentNode.GetChildren();

            if (childIDs == null)
                return null;
            else if (childIDs.Count == 1)
            {
                //If there's only one option, then it goes to the next.
                return theDialogue.GetNodeByID(childIDs[0]);

            }
            else if (childIDs.Count > 1)
            {
                //Need the player to make a selection.
                //For now, we'll return null.
                return null;
            }
            else
                return null;
        }
    }
}