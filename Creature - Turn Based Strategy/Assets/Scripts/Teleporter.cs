using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform GrassTarget;
    public Transform WaterTarget;
    public Transform SandTarget;
    public Transform FireTarget;
    public Rigidbody player;

    int currentAction;


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        Teleport();
    //    }
    //}


    //public void Teleport()
    //{
    //    player.transform.position = teleportTarget.position;
    //}

    void HandleTeleportAction()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        //dialogueBox.UpdateActionSelection(currentAction);

        //change states either move list or run away
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //0 = Teleport to this tower
                player.transform.position = GrassTarget.position;
            }
            else if (currentAction == 1)
            {
                //1 = Teleport to this tower
                player.transform.position = WaterTarget.position;
            }
            else if (currentAction == 2)
            {
                //2 = Teleport to this tower
                player.transform.position = SandTarget.position;
            }
            else if (currentAction == 3)
            {
                //3 = Teleport to this tower
                player.transform.position = FireTarget.position;
            }
        }
    }
}
