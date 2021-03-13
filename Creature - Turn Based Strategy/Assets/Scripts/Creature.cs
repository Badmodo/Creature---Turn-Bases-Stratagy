using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature 
{
    //pickup fron the base class
    public CreatureBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }

    //list of moves for the creature
    public List<Move> Moves { get; set; }

    public Creature(CreatureBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        //checks level and moves available at that level
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            //creature can only have 4 moves, this limits that
            if(Moves.Count >= 4)
            {
                break;
            }
        }
    }

    public int Attack
    {
        get 
        { 
            return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; 
        }
    }
    public int Defense
    {
        get
        {
            return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5;
        }
    }
    public int SpecialAttack
    {
        get
        {
            return Mathf.FloorToInt((Base.SpecialAttack * Level) / 100f) + 5;
        }
    }
    public int SpecialDefense
    {
        get
        {
            return Mathf.FloorToInt((Base.SpecialDefense * Level) / 100f) + 5;
        }
    }
    public int Speed
    {
        get
        {
            return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5;
        }
    }
    public int MaxHp
    {
        get
        {
            return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
        }
    }


}
