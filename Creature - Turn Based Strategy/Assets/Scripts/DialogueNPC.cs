using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;

    public void Interact()
    {
        //Debug.Log("Even Weirder this is working");
        //singleton pattern. Be careful of dependancies
        StartCoroutine(DialogueManager.Instance.ShowDialog(dialogue));
    }
}
