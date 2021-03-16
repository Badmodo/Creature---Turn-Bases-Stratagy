using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureTeam : MonoBehaviour
{
    [SerializeField] List<Creature> creatures;

    public List<Creature> Creatures
    {
        get
        {
            return creatures;
        }
    }

    private void Start()
    {
        foreach(var creature in creatures)
        {
            creature.Initialisation();
        }
    }

    //when called return the first creature that is not Fainted
    public Creature GetHealthyCreature()
    {
        //origianlly used a for loop to find a creature that had not fainted. Linq used instead
        return creatures.Where(x => x.HP > 0).FirstOrDefault();
    }

    //this functionallity add creature to your party
    public void AddCreature(Creature newCreature)
    {
        if(creatures.Count < 6)
        {
            creatures.Add(newCreature);
        }
        else
        {
            //SAM - we dont dont have a PC yet - SAM
            //CREATURES ARE LOST IN SPACE
        }
    }
}
