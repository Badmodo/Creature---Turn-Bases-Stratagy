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
    //Disctionary to store Stat manipulators
    public Dictionary<Stat, int> StatBoost { get; private set; }
    //set this status on the creature
    public Condition Status { get; set; }
    //queue is like a list but you can take things out in the order you put them in, it also needs to be initilsed
    public Queue<string> StatusChange { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }

    public void Initialisation() /*public Creature(CreatureBase pBase, int pLevel)*/
    {
        //these will now be called from the constructor not manually set
        //Base = pBase;
        //Level = pLevel;

        //checks level and moves available at that level
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            //creature can only have 4 moves, this limits that
            if (Moves.Count >= 4)
            {
                break;
            }
        }
        CalculateStats();
        HP = MaxHp;

        ResetStatBoost();
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

    //creatures need there stat boosts to be reset after each battle
    void ResetStatBoost()
    {
        //on initilize all stat boosts are set to 0 in the dictionary
        StatBoost = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpecialAttack, 0 },
            { Stat.SpecialDefence, 0 },
            { Stat.Speed, 0 },
        };
    }

    //this function will take a value of the stat and will take a stat enum as a paramater
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //at this point we can add stat boosts and declines
        int boost = StatBoost[stat];
        //these values are the same as calculated in pokemon
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >=0)
        {
            //change it to an int 
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            //change it to an int 
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }

    //applciation of the boost features
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            //stats cant be boosted or dulled for more than 6 places so clamp
            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -6, 6);

            //shows a stat change in dialogue. Enqueue is to add to a queue
            if(boost > 0)
            {
                StatusChange.Enqueue($"{Base.Name}'s {stat} rose!");
            }
            else
            {
                StatusChange.Enqueue($"{Base.Name}'s {stat} fell!");
            }
        }
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
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpecialAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpecialDefense : Defense;

        //modifiers including random range, type bonus and critical bonus
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        //HP -= damage;
        //if(HP <= 0)
        //{
        //    HP = 0;
        //    damageDetails.Fainted = true;
        //}
        UpdateHP(damage);

        return damageDetails;
    }

    //take the damamge as the paramater, clamp it so it dosnt go beneath 0
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    //function to set the status and use dialogue box to show status change
    public void SetStatus(ConditionsID conditionsID)
    {
        Status = ConditionsDB.Conditions[conditionsID];
        StatusChange.Enqueue($"{Base.Name}{Status.StartMessage}");
    }

    //this creates a random move from the enemy creature when its their attack phase
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    //this is going to be used a lot in the future with updates causing a number of status issues for creatures
    //we will only call this if it is not null ?.invoke
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }

    //called when the battle is over
    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}