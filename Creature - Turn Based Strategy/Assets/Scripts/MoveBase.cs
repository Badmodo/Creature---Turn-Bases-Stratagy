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
    [SerializeField] bool cantMiss;
    [SerializeField] int pp;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    //properties to expose all the varibales
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
    public bool CantMiss
    {
        get
        {
            return cantMiss;
        }
    }
    public int Pp
    {
        get
        {
            return pp;
        }
    }
    public MoveCategory Category
    {
        get
        {
            return category;
        }
    }
    public MoveEffects Effects
    {
        get
        {
            return effects;
        }
    }
    public MoveTarget Target
    {
        get
        {
            return target;
        }
    }

    //replaced this with an enum
    //this designates if a move uses attack or special attack, public bool IsSpecial
    //{
    //    get
    //    {
    //        if(type == CreatureType.Fire || type == CreatureType.Water || type == CreatureType.Grass || type == CreatureType.Ice || type == CreatureType.Electric || type == CreatureType.Dragon)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //}
}

//specify the stats that can be boosted by this move
//tried using a Dictionary my new favourite but you cant serizlie a dictionary
[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionsID status;
    [SerializeField] ConditionsID volitileStatus;

    public List<StatBoost> Boosts
    {
        get
        {
            return boosts;
        }
    }
    public ConditionsID Status
    {
        get
        {
            return status;
        }
    }
    public ConditionsID VolitileStatus
    {
        get
        {
            return volitileStatus;
        }
    }
}

//only purpose of this class is to be shown as a list in MoveEffects
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

//physical touches the enemy, special used special moves to damage and status effects the stats
public enum MoveCategory
{
    Physical,
    Special,
    Status
}

//this dictates if you apply the boost to yourself or your foe
public enum  MoveTarget
{
    foe, self
}
