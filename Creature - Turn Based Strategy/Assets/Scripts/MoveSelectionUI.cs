using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highLightColor;

    int currentSelection = 0;


    //take the list of current moves and a reference to a new move
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        //set the names of the current moves
        for(int i = 0; i< currentMoves.Count; ++i)
        {
            //set the list of the names to the current 
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    //using arrow up and down you can nanvigate the move select menu
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentSelection;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentSelection;
        }
        currentSelection = Mathf.Clamp(currentSelection, 0, Creature.MaxNumberOfMoves);

        UpdateMoveSelection(currentSelection);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            //able to use paramaters from with battlesystem using action int onSleceted
            onSelected.Invoke(currentSelection);

        }
    }
    
    //in updating it removes unwated skill
    public void UpdateMoveSelection(int selection)
    {
        for(int i = 0; i < Creature.MaxNumberOfMoves; i++)
        {
            if(i == selection)
            {
                moveTexts[i].color = highLightColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}
