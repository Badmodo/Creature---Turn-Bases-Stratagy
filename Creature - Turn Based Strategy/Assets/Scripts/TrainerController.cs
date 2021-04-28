using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    //[SerializeField] Dialogue dialogue;
    [SerializeField] GameObject SeePlayer; 
    [SerializeField] Sprite sprite;
    [SerializeField] new string name;

    public IEnumerator TriggerTrainerBattle()
    {
        SeePlayer.SetActive(true);
        yield return new WaitForSeconds(.5f);
        SeePlayer.SetActive(false);

        ////if I need dialogue
        //StartCoroutine(DialogueManager.Instance.ShowDialog(dialogue, () =>
        //{
        //    Debug.Log("start trainer battle");
        //}));
    }

    public string Name
    { get => name; }

    public Sprite Sprite
    { get => sprite; }
}
