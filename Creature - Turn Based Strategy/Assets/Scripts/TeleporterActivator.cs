using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterActivator : MonoBehaviour
{
    public static bool canTeleport = false;

    private void OnTriggerEnter(Collider other)
    {
        canTeleport = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canTeleport = false;
    }   
}
