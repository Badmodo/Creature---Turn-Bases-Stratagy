using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to create directly from the Project menu
[CreateAssetMenu(fileName = "Creature", menuName = "Creature/Create new Creature")]
public class CreatureBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] CreatureType type1;
    [SerializeField] CreatureType type2;

    // Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int specialAttack;
    [SerializeField] int specialDefense;
    [SerializeField] int speed;

    [SerializeField] int catchRate = 255;

    //[SerializeField] int expYield;
    //[SerializeField] GrowthRate growthRate;

    //[SerializeField] List<LearnableMove> learnableMoves;


    //Decided not to use a function like this
    //public string GetName()
    //{
    //    retun name;
    //}

    //using property instead, these allow me to pick up these variables in other scripts
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    public CreatureType Type1
    {
        get { return type1; }
    }
    public CreatureType Type2
    {
        get { return type2; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int SpecialAttack
    {
        get { return specialAttack; }
    }
    public int SpecialDefense
    {
        get { return specialDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
    //public int ExpYield
    //{
    //    get { return expYield; }
    //}
    //public GrowthRate GrowthRate
    //{
    //    get { return growthRate; }
    //}
}

//Types to show up in drop down for type select
public enum CreatureType
{
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon
}
