using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterActivator : MonoBehaviour
{
    public static bool canTeleport = false;

    [SerializeField] private GameObject pressZToTeleportText;

    private void OnTriggerEnter(Collider other)
    {
        canTeleport = true;
        pressZToTeleportText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        canTeleport = false;
        pressZToTeleportText.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        canTeleport = true;
    }
}
