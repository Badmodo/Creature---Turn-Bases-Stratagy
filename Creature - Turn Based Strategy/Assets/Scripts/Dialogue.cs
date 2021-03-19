using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    //creates string lines enterable in edit mode
    [SerializeField] List<string> lines;

    //this script is designed to expand later on if we have quests or any fun thigns we want to add in

    public List<string> Lines
    {
        get
        {
            return lines;
        }
    }
}
