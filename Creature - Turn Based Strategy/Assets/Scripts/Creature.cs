using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ System.Serializable]
public class Creature 
{
    [SerializeField] CreatureBase _base;
    [SerializeField] int level;

    //pickup fron the base class, now a property
    public CreatureBase Base 
    {
        get
        { 
            return _base; 
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public int HP { get; set; }

    //list of moves for the creature
    public List<Move> Moves { get; set; }
    //public dictionary to store stats! private so it can only be changed in this class
    //dictionary stors a key as well as the value
    public Dictionary<Stat, int> Stats { get; private set; }

    public void Initialisation() /*public Creature(CreatureBase pBase, int pLevel)*/
    {
        //these will now be called from the constructor not manually set
        //Base = pBase;
        //Level = pLevel;

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
        CalculateStats();
        HP = MaxHp;
    }

    //calculate the value of all stats and store them in the Dictinary
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpecialAttack, Mathf.FloorToInt((Base.SpecialAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpecialDefence, Mathf.FloorToInt((Base.SpecialDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
    }

    //this function will take a value of the stat and will take a stat enum as a paramater
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //SAM - at this point we can add stat boosts and declines

        return statVal;
    }

    //all stats for each creature
    public int Attack
    {
        get 
        { 
            return GetStat(Stat.Attack); 
        }
    }
    public int Defense
    {
        get
        {
            return GetStat(Stat.Defense);
        }
    }
    public int SpecialAttack
    {
        get
        {
            return GetStat(Stat.SpecialAttack);
        }
    }
    public int SpecialDefense
    {
        get
        {
            return GetStat(Stat.SpecialDefence);
        }
    }
    public int Speed
    {
        get
        {
            return GetStat(Stat.Speed);
        }
    }
    public int MaxHp { get; private set; }

    //this calculation is the same as the one preformed in a pokemon game, complicated. Yes.
    public DamageDetails TakeDamage(Move move, Creature attacker)
    {
        //crit hits happen only 6.25 percent of the time
        float critical = 1f;
        if(Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        //uses the type chart to get effectivenss of attacks
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        //conditional operator, in place of an if else
        float attack = (move.Base.IsSpecial) ? attacker.SpecialAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? SpecialDefense : Defense;

        //modifiers including random range, type bonus and critical bonus
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    //this creates a random move from the enemy creature when its their attack phase
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}