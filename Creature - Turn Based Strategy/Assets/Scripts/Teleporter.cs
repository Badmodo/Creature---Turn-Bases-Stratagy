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

    public GameObject GrassBits;
    public GameObject WaterBits;
    public GameObject SandBits;
    public GameObject FireBits;

    public GameObject GrassBackground;
    public GameObject WaterBackground;
    public GameObject SandBackground;
    public GameObject FireBackground;

    int currentAction;

    [SerializeField] BattleDialogueBox dialogueBox;


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

    private void Update()
    {
        if(GrassBits == true)
        {
            GrassBackground.SetActive(true);
            WaterBackground.SetActive(false);
            SandBackground.SetActive(false);
            FireBackground.SetActive(false);

            WaterBits.SetActive(false);
            SandBits.SetActive(false);
            FireBits.SetActive(false);
        }
        else if(WaterBits == true)
        {
            GrassBackground.SetActive(false);
            WaterBackground.SetActive(true);
            SandBackground.SetActive(false);
            FireBackground.SetActive(false);

            GrassBits.SetActive(false);
            SandBits.SetActive(false);
            FireBits.SetActive(false);
        }
        else if(SandBits == true)
        {
            GrassBackground.SetActive(false);
            WaterBackground.SetActive(false);
            SandBackground.SetActive(true);
            FireBackground.SetActive(false);

            GrassBits.SetActive(false);
            WaterBits.SetActive(false);
            FireBits.SetActive(false);
        }
        else if(FireBits == true)
        {
            GrassBackground.SetActive(false);
            WaterBackground.SetActive(false);
            SandBackground.SetActive(false);
            FireBackground.SetActive(true);

            GrassBits.SetActive(false);
            SandBits.SetActive(false);
            WaterBits.SetActive(false);
        }
    }

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

        dialogueBox.UpdateActionSelection(currentAction);

        //change states either move list or run away
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //0 = Teleport to this tower
                GrassBits.SetActive(true);
                player.transform.position = GrassTarget.position;
            }
            else if (currentAction == 1)
            {
                //1 = Teleport to this tower
                WaterBits.SetActive(true);
                player.transform.position = WaterTarget.position;
            }
            else if (currentAction == 2)
            {
                //2 = Teleport to this tower
                SandBits.SetActive(true);
                player.transform.position = SandTarget.position;
            }
            else if (currentAction == 3)
            {
                //3 = Teleport to this tower
                FireBits.SetActive(true);
                player.transform.position = FireTarget.position;
            }
        }
    }
}
