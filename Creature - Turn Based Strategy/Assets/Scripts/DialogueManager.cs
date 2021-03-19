using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogueText;
    [SerializeField] int lettersPerSecond;

    //event to stop player movement
    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    //static dialogue manager class can be referenced anywhere we want
    public static DialogueManager Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    //store dialogue in global object outside showdialogue
    Dialogue dialogue;
    //check current line of dialogue
    int currentLine = 0;
    //stops you putting works mixed in with others
    bool isTyping;

    public IEnumerator ShowDialog(Dialogue dialogue)
    {
        //wait for one frame allows the handleupdate to happen stopping the !isTyping issues
        yield return new WaitForEndOfFrame();
        
        OnShowDialogue.Invoke();

        this.dialogue = dialogue;
        dialogueBox.SetActive(true);
        //show the first line of dialogue
        StartCoroutine(TypeDialog(dialogue.Lines[0]));
    }

    public void HandleUpdate()
    {
        //&& is to stop the text being overweritten and adding together when typing
        if(Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialogue.Lines[currentLine]));
            }
            else
            {
                //reset lines so next dialogue it starts at 0 
                currentLine = 0;
                dialogueBox.SetActive(false);
                //invoke the on close dialogue state control
                OnCloseDialogue?.Invoke();
            }
        }
    }

    //make the dialogue text appear as if typed
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;

        //setting the text to an empty string
        dialogueText.text = "";
        //loop through each letter of the dialogue
        foreach (var letter in line.ToCharArray())
        {
            //add it one by one
            dialogueText.text += letter;
            //type dialogue slowly
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isTyping = false;
    }
}
