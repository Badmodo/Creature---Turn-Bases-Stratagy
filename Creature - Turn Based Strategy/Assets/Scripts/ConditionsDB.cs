using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    //creating the conditions dictionary and initilising it
    public static Dictionary<ConditionsID, Condition> Conditions { get; set; } = new Dictionary<ConditionsID, Condition>() 
    {
        {
            ConditionsID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has beem poisoned",
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
                StartMessage = "has beem burnt",
                OnAfterTurn = (Creature creature) =>
                {
                    //a burnt creature takes 16% damage per turn 
                    creature.UpdateHP(creature.MaxHp / 16);
                    creature.StatusChange.Enqueue($"{creature.Base.Name} hurt itself due to burn");
                }
            }
        }    
    };
}

public enum ConditionsID
{
    none, psn, brn, slp, par, frz
}
