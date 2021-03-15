using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static void Initilize()
    {
        //loop through both key and value in conditions
        foreach(var KeyValuePairs in Conditions)
        {
            //making var from the conditionsID dictionary
            var conditionId = KeyValuePairs.Key;
            var condition = KeyValuePairs.Value;

            //asign id from conditions class here & will be auto set for all conditions
            condition.Id = conditionId;
        }    
    }

    //creating the conditions dictionary and initilising it
    public static Dictionary<ConditionsID, Condition> Conditions { get; set; } = new Dictionary<ConditionsID, Condition>() 
    {
        {
            ConditionsID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = " has beem poisoned",
                OnAfterTurn = (Creature creature) =>
                {
                    //a poisoned creature takes 8% damage per turn 
                    creature.UpdateHP(creature.MaxHp / 8);
                    creature.StatusChange.Enqueue($"{creature.Base.Name} hurt itself due to poison");
                }
            }
        },    
        {
            ConditionsID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = " has beem burnt",
                OnAfterTurn = (Creature creature) =>
                {
                    //a burnt creature takes 16% damage per turn 
                    creature.UpdateHP(creature.MaxHp / 16);
                    creature.StatusChange.Enqueue($"{creature.Base.Name} hurt itself due to burn");
                }
            }
        },    
        {
            ConditionsID.par,
            new Condition()
            {
                Name = "Paralyze",
                StartMessage = " has been paralyzed",
                OnBeforeMove = (Creature Creature) =>
                {
                    //1 out of 4 times the creature will not be able to use the move
                    if(Random.Range(1, 5) == 1)
                    {
                        Creature.StatusChange.Enqueue($"{Creature.Base.Name}'s paralyzed and can't move");
                        return false;
                    }
                    //move goes off fine
                    return true;
                }
            }
        },    
        {
            ConditionsID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = " has been frozen",
                OnBeforeMove = (Creature creature) =>
                {
                    //1 out of 4 times the creature will cure its frozen status
                    if(Random.Range(1, 5) == 1)
                    {
                        creature.CureStatus();
                        creature.StatusChange.Enqueue($"{creature.Base.Name}'s not frozen anymore");
                        return true;
                    }
                    //move goes off fine
                    return false;
                }
            }
        },    
        {
            ConditionsID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = " has fallen asleep",
                OnStart = (Creature creature) =>
                {
                    //sleep for 1-3 turns
                    creature.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {creature.StatusTime} moves");
                },
                OnBeforeMove = (Creature creature) =>
                {
                    if(creature.StatusTime <= 0)
                    {
                        creature.CureStatus();
                        creature.StatusChange.Enqueue($"{creature.Base.Name} woke up");
                        return true;
                    }

                    creature.StatusTime--;
                    creature.StatusChange.Enqueue($"{creature.Base.Name} is sleeping");
                    //move goes off fine
                    return false;
                    
                }
            }
        }    
    };
}

public enum ConditionsID
{
    none, psn, brn, slp, par, frz
}
