using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this creates the states in which the battle can be in
public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, BattleTeamScreen, BattleOverKO}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    //removed to streamline code and run it from the BattleUnit script
    //[SerializeField] BattleHud playerHud;
    //[SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogueBox dialogueBox;
    [SerializeField] BattleTeamScreen battleTeamScreen;

    public event Action<bool> BattleOver;

    BattleState state;

    int currentAction;
    int currentMove;
    int currentMember;

    CreatureTeam playerteam;
    Creature wildCreature;

    public void StartBattle(CreatureTeam playerteam, Creature wildCreature)
    {
        //wierd use of the same name just to make it work in script
        this.playerteam = playerteam;
        this.wildCreature = wildCreature;
        StartCoroutine(SetUpBattle());
    }

    //simp;le set up to show player and enemy set up
    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup(playerteam.GetHealthyCreature());
        enemyUnit.Setup(wildCreature);
        //Now being called from BattleUnit
        //playerHud.SetData(playerUnit.Creature);
        //enemyHud.SetData(enemyUnit.Creature);

        battleTeamScreen.Initilised();

        //Passing the creatures moves to the set moves function
        dialogueBox.SetMoveNames(playerUnit.Creature.Moves);

        //using string interprilation to bring in game spacific text
        yield return dialogueBox.TypeDialog($"A wild {enemyUnit.Creature.Base.Name} appeared");

        //wait for a second and then allow the player to choose the next action
        ChooseFirstTurn();
    }

    //determins based on speed who goes first
    void ChooseFirstTurn()
    {
        if(playerUnit.Creature.Speed >= enemyUnit.Creature.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    //state called when you knock out your enemy
    void BattleOverKO(bool won)
    {
        //battloverKo is the state
        state = BattleState.BattleOverKO;
       //short way to write a foreach using linq
        playerteam.Creatures.ForEach(p => p.OnBattleOver());
        //event that shows the battle is over
        BattleOver(won);
    }

    //state where you choose what action to do
    void ActionSelection()
    {
        //change state
        state = BattleState.ActionSelection;
        dialogueBox.SetDialog("Choose an action");
        dialogueBox.EnableActionText(true);
    }

    //funtion that opens the Creature Team Screen
    void OpenPartyScreen()
    {
        state = BattleState.BattleTeamScreen;
        battleTeamScreen.SetPartyData(playerteam.Creatures);
        battleTeamScreen.gameObject.SetActive(true);
    }

    //what happens to the dialoguebox when they attack
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionText(false);
        dialogueBox.EnableDialogText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    //this coroutine selects the currently placed move and returns feedback to the dialogue box
    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Creature.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        //large chunk of code removed and replaced with RunMove()

        //if the battle state was not changed by the RunMove then next step
        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    //This allows for the enemy turn, same formula as the players after the choice. Move is chosen by random
    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Creature.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        //if the battle state was not changed by the RunMove then next step
        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
    }

    //function created to condence similar funtionallity of player and enemy creatures
    //source unit is the one doing the move, the target is recieving it
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //checks to see if the cresture is effected by a modifier that would stop runmove
        bool canRunMove = sourceUnit.Creature.OnBeforeMove();
        if(!canRunMove)
        {
            //yield break is called to stop the coroutine if a the move is unable to run
            yield return ShowStatusChanges(sourceUnit.Creature);
            //to remove hp from damage if required from a volitle status
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Creature);


        //none of this logic will be performed if can run move is false
        move.Pp--;
        yield return dialogueBox.TypeDialog($"{sourceUnit.Creature.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Creature, targetUnit.Creature))
        {
            sourceUnit.BattleAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.BattleHitAmination();

            //check to see if the move was a status move
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move, sourceUnit.Creature, targetUnit.Creature);
            }
            else
            {
                var damageDetails = targetUnit.Creature.TakeDamage(move, sourceUnit.Creature);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            //this coroutine plays if the creature loses all hp
            if (targetUnit.Creature.HP <= 0)
            {
                yield return dialogueBox.TypeDialog($"{targetUnit.Creature.Base.Name} fainted");
                targetUnit.BattleFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        }
        //if the move misses
        else
        {
            yield return dialogueBox.TypeDialog($"{sourceUnit.Creature.Base.Name}'s attack missed");
        }

        //status like poison and burn will hurt the creature after a turn
        sourceUnit.Creature.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Creature);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Creature.HP <= 0)
        {
            yield return dialogueBox.TypeDialog($"{sourceUnit.Creature.Base.Name} fainted");
            sourceUnit.BattleFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    //moved this out of Runmove as there will hopefully be a lot of stats changes in here in the future
    IEnumerator RunMoveEffects(Move move, Creature source, Creature target)
    {
        var effects = move.Base.Effects;

        //stat boosting
        if (effects.Boosts != null)
        {
            //see if the move is targeting what creature, self boost or decline on enemy
            if (move.Base.Target == MoveTarget.self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        //check if this move will cause a status condition
        if(effects.Status != ConditionsID.none)
        {
            target.SetStatus(effects.Status);
        }
        
        //check if this move will cause a Volitilestatus condition
        if(effects.VolitileStatus != ConditionsID.none)
        {
            target.SetVolitileStatus(effects.VolitileStatus);
        }

        //now we apply the boost we show the status function to either
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    //this will determine if a move will hit
    bool CheckIfMoveHits(Move move, Creature source, Creature target)
    {
        if(move.Base.CantMiss)
        {
            return true;
        }
        //typical move accuraity is 100
        float moveAccuracy = move.Base.Accuracy;
        //boost the move accuracy and evasion stats
        int accuracy = source.StatBoost[Stat.Accuracy];
        int evasion = target.StatBoost[Stat.Evasion];

        //Boost values for Accuraicy
        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        //accuracy adjuster
        if(accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        //evasion adjuster
        if(evasion > 0)
        {
            moveAccuracy /= boostValues[evasion];
        }
        else
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    //to remove a message from a queue you need to use dequeue 
    IEnumerator ShowStatusChanges(Creature creature)
    {
        while(creature.StatusChange.Count > 0)
        {
            var message = creature.StatusChange.Dequeue();
            yield return dialogueBox.TypeDialog(message);
        }
    }

    //what happens if the target unit fainted
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if(faintedUnit.IsPlayerUnit)
        {
            var nextCreature = playerteam.GetHealthyCreature();
            if (nextCreature != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOverKO(false);
            }
        }
        else
        {
            BattleOverKO(true);
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
    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.BattleTeamScreen)
        {
            HandleBattleTeamSelection();
        }
    }

    //working out how to navigate the action options
    void HandleActionSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);
            
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    if (currentAction < 1)
        //    {
        //        ++currentAction;
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    if (currentAction > 0)
        //    {
        //        --currentAction;
        //    }
        //}

        dialogueBox.UpdateActionSelection(currentAction);

        //change states either move list or run away
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                //0 = fight state
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //1 = bag state
            }
            else if (currentAction == 2)
            {
                //2 = creature state
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //3 = run state
            }
        }
    }

    //there are 4 moves potenatially, this is designed to naviagate all of them
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Creature.Moves.Count - 1);

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Creature.Moves[currentMove]);

        //we now select a move. disable the selector. damage eney and change state
        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    //logic of moving around the creature select screen
    void HandleBattleTeamSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerteam.Creatures.Count - 1);

        battleTeamScreen.UpdateMemeberSelection(currentMember);

        //used to select the current creature
        if(Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerteam.Creatures[currentMember];
            //checks if the selected creature has fainted
            if(selectedMember.HP <= 0)
            {
                battleTeamScreen.SetMessageText("You can't send out a fainted Creature");
                return;
            }
            if(selectedMember == playerUnit.Creature)
            {
                battleTeamScreen.SetMessageText("This creature is already out");
                return;
            }
            //disables select screen, sets to busy and start switch
            battleTeamScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchCreature(selectedMember));
        }
        //to back out press x
        else if(Input.GetKeyDown(KeyCode.X))
        {
            battleTeamScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    //used to switch creatures
    IEnumerator SwitchCreature(Creature newCreature)
    {
        bool currentCreatureFainted = true;

        if (playerUnit.Creature.HP > 0)
        {
            currentCreatureFainted = false;
            //call out to your creature Return *friend*
            yield return dialogueBox.TypeDialog($"Return {playerUnit.Creature.Base.Name}");
            //SAM - replace below faint animation with return animation - SAM
            playerUnit.BattleFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        //send out new creture
        playerUnit.Setup(newCreature);
        //Passing the next creatures moves to the set moves function over the previous
        dialogueBox.SetMoveNames(newCreature.Moves);

        //using string interprilation to bring in game spacific text
        yield return dialogueBox.TypeDialog($"Go {newCreature.Base.Name}");

        if (currentCreatureFainted)
        {
            ChooseFirstTurn();
        }
        else
        {
            //players turn over, enemy turn
            StartCoroutine(EnemyMove());
        }
    }
}

