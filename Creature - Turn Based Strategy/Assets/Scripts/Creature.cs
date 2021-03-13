using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature 
{
    //pickup fron the base class
    CreatureBase _base;
    int level;


    public Creature(CreatureBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
    }

    public int Attack
    {
        get 
        { 
            return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5; 
        }
    }
    public int Defense
    {
        get
        {
            return Mathf.FloorToInt((_base.Defense * level) / 100f) + 5;
        }
    }
    public int SpecialAttack
    {
        get
        {
            return Mathf.FloorToInt((_base.SpecialAttack * level) / 100f) + 5;
        }
    }
    public int SpecialDefense
    {
        get
        {
            return Mathf.FloorToInt((_base.SpecialDefense * level) / 100f) + 5;
        }
    }
    public int Speed
    {
        get
        {
            return Mathf.FloorToInt((_base.Speed * level) / 100f) + 5;
        }
    }
    public int MaxHp
    {
        get
        {
            return Mathf.FloorToInt((_base.MaxHp * level) / 100f) + 10;
        }
    }


}
