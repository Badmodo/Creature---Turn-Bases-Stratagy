using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform GrassTarget;
    public Transform WaterTarget;
    public Transform DesertTarget;
    public Transform FireTarget;
    
    public GameObject player;
    public GameObject TeleportScreen;

    public GameObject GrassPlanet;
    public GameObject WaterPlanet;
    public GameObject DesertPlanet;
    public GameObject FirePlanet;

    public GameObject GrassBits;
    public GameObject WaterBits;
    public GameObject DesertBits;
    public GameObject FireBits;

    public GameObject GrassBackground;
    public GameObject WaterBackground;
    public GameObject DesertBackground;
    public GameObject FireBackground;

    public static Vector3 GrassPlayerSpawnPosition = new Vector3(-3.4f, 10f, 111f);
    public static Vector3 WaterPlayerSpawnPosition = new Vector3(146f, -86f, -2f);
    public static Vector3 DesertPlayerSpawnPosition = new Vector3(-130f, 94.5f, 7f);
    public static Vector3 FirePlayerSpawnPosition = new Vector3(16f, 1f, -119f);

    public static Vector3 GrassPlayerSpawnEulerAngles = new Vector3(-1f, -42f, 75f);
    public static Vector3 WaterPlayerSpawnEulerAngles = new Vector3(185f, -51f, -1f);
    public static Vector3 DesertPlayerSpawnEulerAngles = new Vector3(180f, 2.3f, 127f);
    public static Vector3 FirePlayerSpawnEulerAngles = new Vector3(188f, -1f, 104f);

    public enum PlanetType
    {
        Grass,
        Water,
        Desert,
        Fire
    }

    int currentAction;

    private PlanetGravity planetGravity;

    [SerializeField] BattleDialogueBox dialogueBox;


    private void Start()
    {

    }


    private void Update()
    {

        //switch (planetSelector)
        //{
        //    case PlanetSelector.Grass:
        //        {
        //            GrassBackground.SetActive(true);
        //            WaterBackground.SetActive(false);
        //            SandBackground.SetActive(false);
        //            FireBackground.SetActive(false);

        //            GrassBits.SetActive(true);
        //            WaterBits.SetActive(false);
        //            SandBits.SetActive(false);
        //            FireBits.SetActive(false);

        //            GrassPlanet.gameObject.tag = "Planet";
        //            WaterPlanet.gameObject.tag = "Untagged";
        //            SandPlanet.gameObject.tag = "Untagged";
        //            FirePlanet.gameObject.tag = "Untagged";

        //            break;
        //        }
        //    case PlanetSelector.Water:
        //        {
        //            GrassBackground.SetActive(false);
        //            WaterBackground.SetActive(true);
        //            SandBackground.SetActive(false);
        //            FireBackground.SetActive(false);

        //            GrassBits.SetActive(false);
        //            WaterBits.SetActive(true);
        //            SandBits.SetActive(false);
        //            FireBits.SetActive(false);

        //            GrassPlanet.gameObject.tag = "Untagged";
        //            WaterPlanet.gameObject.tag = "Planet";
        //            SandPlanet.gameObject.tag = "Untagged";
        //            FirePlanet.gameObject.tag = "Untagged";
        //            break;
        //        }
        //    case PlanetSelector.Desert:
        //        {
        //            GrassBackground.SetActive(false);
        //            WaterBackground.SetActive(false);
        //            SandBackground.SetActive(true);
        //            FireBackground.SetActive(false);

        //            GrassBits.SetActive(false);
        //            WaterBits.SetActive(false);
        //            SandBits.SetActive(true);
        //            FireBits.SetActive(false);
        //            break;
        //        }
        //    case PlanetSelector.Fire:
        //        {
        //            GrassBackground.SetActive(false);
        //            WaterBackground.SetActive(false);
        //            SandBackground.SetActive(false);
        //            FireBackground.SetActive(true);

        //            GrassBits.SetActive(false);
        //            SandBits.SetActive(false);
        //            WaterBits.SetActive(false);
        //            FireBits.SetActive(true);
        //            break;
        //        }
        //    default:
        //        {

        //            break;
        //        }

        }


    public void TeleportToGrassPlanet()
    {
        Teleport(PlanetType.Grass);
    }

    public void TeleportToWaterPlanet()
    {
        //StartCoroutine(Teleport(PlanetType.Water));
        Teleport(PlanetType.Water);
    }

    public void TeleportToDesertPlanet()
    {
        Teleport(PlanetType.Desert);
    }

    public void TeleportToFirePlanet()
    {
        Teleport(PlanetType.Fire);
    }

    public void Teleport(PlanetType _planetType)
    {
        TeleportScreen.SetActive(false);
        Time.timeScale = 1f;

        switch (_planetType)
        {
            case PlanetType.Grass:
                GrassBackground.SetActive(true);
                WaterBackground.SetActive(false);
                DesertBackground.SetActive(false);
                FireBackground.SetActive(false);

                GrassBits.SetActive(true);
                WaterBits.SetActive(false);
                DesertBits.SetActive(false);
                FireBits.SetActive(false);

                GrassPlanet.gameObject.tag = "Planet";
                WaterPlanet.gameObject.tag = "Untagged";
                DesertPlanet.gameObject.tag = "Untagged";
                FirePlanet.gameObject.tag = "Untagged";

                //yield return new WaitForSeconds(0.5f);

                PlayerGravity.UpdatePlanetGravity();
                PlayerController360.planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();

                player.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                player.transform.position = GrassPlayerSpawnPosition;
                player.transform.eulerAngles = GrassPlayerSpawnEulerAngles;
                break;

            case PlanetType.Water:
                GrassBackground.SetActive(false);
                WaterBackground.SetActive(true);
                DesertBackground.SetActive(false);
                FireBackground.SetActive(false);

                GrassBits.SetActive(false);
                WaterBits.SetActive(true);
                DesertBits.SetActive(false);
                FireBits.SetActive(false);

                GrassPlanet.gameObject.tag = "Untagged";
                WaterPlanet.gameObject.tag = "Planet";
                DesertPlanet.gameObject.tag = "Untagged";
                FirePlanet.gameObject.tag = "Untagged";

                //yield return new WaitForSeconds(0.5f);

                PlayerGravity.UpdatePlanetGravity();
                planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();

                player.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                player.transform.position = WaterPlayerSpawnPosition;
                player.transform.eulerAngles = WaterPlayerSpawnEulerAngles;
                break;

            case PlanetType.Desert:
                GrassBackground.SetActive(false);
                WaterBackground.SetActive(false);
                DesertBackground.SetActive(true);
                FireBackground.SetActive(false);

                GrassBits.SetActive(false);
                WaterBits.SetActive(false);
                DesertBits.SetActive(true);
                FireBits.SetActive(false);

                GrassPlanet.gameObject.tag = "Untagged";
                WaterPlanet.gameObject.tag = "Untagged";
                DesertPlanet.gameObject.tag = "Planet";
                FirePlanet.gameObject.tag = "Untagged";

                //yield return new WaitForSeconds(0.5f);

                PlayerGravity.UpdatePlanetGravity();
                planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();

                player.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                player.transform.position = DesertPlayerSpawnPosition;
                player.transform.eulerAngles = DesertPlayerSpawnEulerAngles;
                break;

            case PlanetType.Fire:
                GrassBackground.SetActive(false);
                WaterBackground.SetActive(false);
                DesertBackground.SetActive(false);
                FireBackground.SetActive(true);

                GrassBits.SetActive(false);
                WaterBits.SetActive(false);
                DesertBits.SetActive(false);
                FireBits.SetActive(true);

                GrassPlanet.gameObject.tag = "Untagged";
                WaterPlanet.gameObject.tag = "Untagged";
                DesertPlanet.gameObject.tag = "Untagged";
                FirePlanet.gameObject.tag = "Planet";

                //yield return new WaitForSeconds(0.5f);

                PlayerGravity.UpdatePlanetGravity();
                planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();

                player.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                player.transform.position = FirePlayerSpawnPosition;
                player.transform.eulerAngles = FirePlayerSpawnEulerAngles;
                break;

            default: break;
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
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    if (currentAction == 0)
        //    {
        //        //0 = Teleport to this tower
        //        planetSelector = 1;
        //        player.transform.position = GrassTarget.position;
        //    }
        //    else if (currentAction == 1)
        //    {
        //        //1 = Teleport to this tower
        //        planetSelector = 2;
        //        player.transform.position = WaterTarget.position;
        //    }
        //    else if (currentAction == 2)
        //    {
        //        //2 = Teleport to this tower
        //        planetSelector = 3;
        //        player.transform.position = SandTarget.position;
        //    }
        //    else if (currentAction == 3)
        //    {
        //        //3 = Teleport to this tower
        //        planetSelector = 4;
        //        player.transform.position = FireTarget.position;
        //    }
        //}
    }
}
