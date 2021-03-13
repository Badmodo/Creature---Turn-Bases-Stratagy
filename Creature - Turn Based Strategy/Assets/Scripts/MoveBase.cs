using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to make the moves from the Project menu
[CreateAssetMenu(fileName = "Creature", menuName = "Creature/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] CreatureType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    //property to expose all the varibales
    public string Name
    {
        get
        {
            return name;
        }
    }
    public string Description
    {
        get
        {
            return description;
        }
    }
    public CreatureType Type
    {
        get
        {
            return type;
        }
    }
    public int Power
    {
        get
        {
            return power;
        }
    }
    public int Accuracy
    {
        get
        {
            return accuracy;
        }
    }
    public int Pp
    {
        get
        {
            return pp;
        }
    }
}
