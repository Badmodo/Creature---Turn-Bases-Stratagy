using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this creates the states in which the battle can be in
public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogueBox dialogueBox;

    BattleState state;

    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetUpBattle());
    }

    //simp;le set up to show player and enemy set up
    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Creature);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Creature);

        //Passing the creatures moves to the set moves function
        dialogueBox.SetMoveNames(playerUnit.Creature.Moves);

        //using string interprilation to bring in game spacific text
        yield return dialogueBox.TypeDialog($"A wild {enemyUnit.Creature.Base.Name} appeared");
        yield return new WaitForSeconds(1f);

        //wait for a second and then allow the player to choose the next action
        PlayerAction();
    }

    //state where you choose what action to do
    void PlayerAction()
    {
        //change state
        state = BattleState.PlayerAction;
        StartCoroutine(dialogueBox.TypeDialog("Choose an action"));
        dialogueBox.EnableActionText(true);
    }

    //
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionText(false);
        dialogueBox.EnableDialogText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    //what to do while in the action select screen
    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    //working out how to navigate the options
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                ++currentAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }

        dialogueBox.UpdateActionSelection(currentAction);

        //change states either move list or run away
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                //0 = fight state
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //1 = run state
            }
        }
    }

    //there are 4 moves potenatially, this is designed to naviagate all of them
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Creature.Moves.Count -1)
            {
                ++currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                --currentMove;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Creature.Moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Creature.Moves[currentMove]);
    }
}
