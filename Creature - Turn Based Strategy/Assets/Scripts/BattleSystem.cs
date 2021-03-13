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

    //what happens to the dialoguebox when they attack
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionText(false);
        dialogueBox.EnableDialogText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    //this coroutine selects the currently placed move and returns feedback to the dialogue box
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Creature.Moves[currentMove];
        yield return dialogueBox.TypeDialog($"{playerUnit.Creature.Base.Name} used {move.Base.Name}");

        playerUnit.BattleAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.BattleHitAmination();

        var damageDetails = enemyUnit.Creature.TakeDamage(move, playerUnit.Creature);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);


        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialog($"{enemyUnit.Creature.Base.Name} fainted");
            enemyUnit.BattleFaintAnimation();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    //This allows for the enemy turn, same formula as the players after the choice. Move is chosen by random
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Creature.GetRandomMove();
        yield return dialogueBox.TypeDialog($"{enemyUnit.Creature.Base.Name} used {move.Base.Name}");

        enemyUnit.BattleAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.BattleHitAmination();

        var damageDetails = playerUnit.Creature.TakeDamage(move, enemyUnit.Creature);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialog($"{playerUnit.Creature.Base.Name} fainted");
            playerUnit.BattleFaintAnimation();
        }
        else
        {
            PlayerAction();
        }
    }

    //this shows in the dialogue box if there was a mulitplier applied to the attack
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        //if critical hit is over 0 it was call this coroutine, same but also with negative for effectiveness
        if(damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialog("A critical hit!");
        }
        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogueBox.TypeDialog("It's super effective!");
        }
        if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialog("It's not very effective!");
        }
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

        //we now select a move. disable the selector. damage eney and change state
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
