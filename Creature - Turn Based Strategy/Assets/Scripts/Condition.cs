using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    //reference to the status conditions ID
    public ConditionsID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    //this is where you can set statustime for stat effecting moves
    public Action<Creature> OnStart { get; set; }
    //causes an action before a creature has its turn, uses a bool, func allows ypu to return a value(sleep)
    public Func<Creature, bool> OnBeforeMove { get; set; }
    //causes an action after a creture has its turn(poison)
    public Action<Creature> OnAfterTurn { get; set; }
}
