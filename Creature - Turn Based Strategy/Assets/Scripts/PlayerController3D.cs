using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{    
    //CharacterController characterController;

    //[SerializeField] Sprite sprite;
    //[SerializeField] string name;

    //public GameObject Player;
    //public float speed = 6.0f;
    //public float jumpSpeed = 8.0f;
    //public float gravity = 20.0f;

    //public event Action onEncounter;

    //public bool inGrass;
    //public bool inDialogue;
    //public bool duringDialogue;

    //public GameObject Battle;

    //private Vector3 moveDirection = Vector3.zero;

    //void Start()
    //{
    //    //Find attached character controllers on player
    //    characterController = GetComponent<CharacterController>();
    //}

    //public void HandleUpdate()
    //{
    //    if (characterController.isGrounded)
    //    {
    //        // We are grounded, so recalculate
    //        // move direction directly from axes
    //        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
    //        moveDirection *= speed;

    //        if (Input.GetButton("Jump"))
    //        {
    //            moveDirection.y = jumpSpeed;
    //        }
    //    }

    //    // Apply gravity
    //    moveDirection.y -= gravity * Time.deltaTime;

    //    // Move the controller
    //    characterController.Move(moveDirection * Time.deltaTime);
    //}

    ////See if the player is in long grass(where enemys hide)
    //void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.tag == "Grass" && !inGrass)
    //    {
    //        inGrass = true;
    //    }

    //    if (inGrass == true)
    //    {
    //        StartCoroutine(EnemyEncounter());
    //    }

    //    //Enemy encounter chance, 1 in 10 every .5 seconds
    //    //we will use a technique called observer design pattern so as to not double call this inside the gamecontroller
    //    IEnumerator EnemyEncounter()
    //    {      
    //        while (inGrass)
    //        {
    //            yield return new WaitForSeconds(.5f);

    //            if (GameController.State == GameState.Freeroam)
    //            {
    //                //had to specifyy the random because system and unity both have a random function
    //                if (UnityEngine.Random.Range(1, 101) <= 10)
    //                {
    //                    //Debug.Log("EnemyEncountered");
    //                    onEncounter();
    //                }
    //                //StartCoroutine(EnemyEncounter());
    //            }
    //        }
    //    }

    //    //Dialogue Collider enter
    //    if (collider.tag == "Dialogue")
    //    {
    //        inDialogue = true;
    //    }

    //    //on collision i am trying to run this. It should clear the dialogue
    //    if (GameController.State == GameState.Freeroam && collider.gameObject.GetComponent<DialogueNPC>())
    //    {
    //        //duringDialogue = true;
    //        collider.GetComponent<Interactable>()?.Interact();
    //    }

    //    //on collision i am trying to run this. It should clear the dialogue
    //    if (GameController.State == GameState.Freeroam && collider.gameObject.GetComponent<TrainerController>())
    //    {
    //        var trainer = collider.GetComponent<TrainerController>();
    //        if (trainer != null)
    //        {
    //            StartCoroutine(trainer.TriggerTrainerBattle());
    //        }
    //    }
    //}


    ////Player no longer in long grass or dialogue
    //void OnTriggerExit(Collider collider)
    //{
    //    if (collider.tag == "Grass" && inGrass)
    //    {
    //        inGrass = false;
    //    }


    //    //SAM - this stops all coroutines, see if it effects launching the fight sequences
    //    //if not in grass stop runnings into 
    //    if (inGrass == false)
    //    {
    //        StopAllCoroutines();
    //    }


    //    //Dialogue Collider exit
    //    if (collider.tag == "Dialogue")
    //    {
    //        inDialogue = false;
    //    }
    //}

    //public string Name
    //{ get => name; }

    //public Sprite Sprite
    //{ get => sprite; }
}
