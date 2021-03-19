using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//in this interface we will define functions but not the actual implamentation
// this allows as an extention after monobehavior of interface, the script must
// implament all functions in this interface
public interface Interactable
{
    void Interact();
}
