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

    public GameObject GrassPlanet;
    public GameObject WaterPlanet;
    public GameObject SandPlanet;
    public GameObject FirePlanet;

    public GameObject GrassBits;
    public GameObject WaterBits;
    public GameObject SandBits;
    public GameObject FireBits;

    public GameObject GrassBackground;
    public GameObject WaterBackground;
    public GameObject SandBackground;
    public GameObject FireBackground;

    public int planetSelector;

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
        int planet = planetSelector;
        switch (planet)
        {
            case 1:
                {
                    GrassBackground.SetActive(true);
                    WaterBackground.SetActive(false);
                    SandBackground.SetActive(false);
                    FireBackground.SetActive(false);

                    GrassBits.SetActive(true);
                    WaterBits.SetActive(false);
                    SandBits.SetActive(false);
                    FireBits.SetActive(false);

                    GrassPlanet.gameObject.tag = "Planet";
                    WaterPlanet.gameObject.tag = "Untagged";
                    SandPlanet.gameObject.tag = "Untagged";
                    FirePlanet.gameObject.tag = "Untagged";

                    break;
                }
            case 2:
                {
                    GrassBackground.SetActive(false);
                    WaterBackground.SetActive(true);
                    SandBackground.SetActive(false);
                    FireBackground.SetActive(false);

                    GrassBits.SetActive(false);
                    WaterBits.SetActive(true);
                    SandBits.SetActive(false);
                    FireBits.SetActive(false);

                    GrassPlanet.gameObject.tag = "Untagged";
                    WaterPlanet.gameObject.tag = "Planet";
                    SandPlanet.gameObject.tag = "Untagged";
                    FirePlanet.gameObject.tag = "Untagged";
                    break;
                }
            case 3:
                {
                    GrassBackground.SetActive(false);
                    WaterBackground.SetActive(false);
                    SandBackground.SetActive(true);
                    FireBackground.SetActive(false);

                    GrassBits.SetActive(false);
                    WaterBits.SetActive(false);
                    SandBits.SetActive(true);
                    FireBits.SetActive(false);
                    break;
                }
            default:
                {
                    GrassBackground.SetActive(false);
                    WaterBackground.SetActive(false);
                    SandBackground.SetActive(false);
                    FireBackground.SetActive(true);

                    GrassBits.SetActive(false);
                    SandBits.SetActive(false);
                    WaterBits.SetActive(false);
                    FireBits.SetActive(true);
                    break;
                }
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
                planetSelector = 1;
                player.transform.position = GrassTarget.position;
            }
            else if (currentAction == 1)
            {
                //1 = Teleport to this tower
                planetSelector = 2;
                player.transform.position = WaterTarget.position;
            }
            else if (currentAction == 2)
            {
                //2 = Teleport to this tower
                planetSelector = 3;
                player.transform.position = SandTarget.position;
            }
            else if (currentAction == 3)
            {
                //3 = Teleport to this tower
                planetSelector = 4;
                player.transform.position = FireTarget.position;
            }
        }
    }
}
