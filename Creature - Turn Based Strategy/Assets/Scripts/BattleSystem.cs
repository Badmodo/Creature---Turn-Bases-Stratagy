using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this creates the states in which the battle can be in
public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, BattleTeamScreen, BattleOverKO}
public enum BattleAction { Move, SwitchCreature, UseItem, Run}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    //removed to streamline code and run it from the BattleUnit script
    //[SerializeField] BattleHud playerHud;
    //[SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogueBox dialogueBox;
    [SerializeField] BattleTeamScreen battleTeamScreen;
    [SerializeField] GameObject captureRing;

    public event Action<bool> BattleOver;

    BattleState state;
    BattleState? priorState;

    int currentAction;
    int currentMove;
    int currentMember;

    CreatureTeam playerteam;
    Creature wildCreature;

    int escapeAttempts;

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

        //set escape attempts to 0
        escapeAttempts = 0;

        battleTeamScreen.Initilised();

        //Passing the creatures moves to the set moves function
        dialogueBox.SetMoveNames(playerUnit.Creature.Moves);

        //using string interprilation to bring in game spacific text
        yield return dialogueBox.TypeDialog($"A wild {enemyUnit.Creature.Base.Name} appeared");

        //wait for a second and then allow the player to choose the next action
        ActionSelection();
    }

    ////determins based on speed who goes first
    //void ChooseFirstTurn()
    //{
    //    if(playerUnit.Creature.Speed >= enemyUnit.Creature.Speed)
    //    {
    //        ActionSelection();
    //    }
    //    else
    //    {
    //        StartCoroutine(EnemyMove());
    //    }
    //}

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

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            //picks the move from the stored move in creature class
            playerUnit.Creature.CurrentMove = playerUnit.Creature.Moves[currentMove];
            enemyUnit.Creature.CurrentMove = enemyUnit.Creature.GetRandomMove();

            //setting creature priority
            int playerMovePriority = playerUnit.Creature.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Creature.CurrentMove.Base.Priority;

            //Check who gets to go first based on speed and manipulators
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            //if there is no priortiy then speed dictates who goes first
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Creature.Speed >= enemyUnit.Creature.Speed;
            }

            var firstUnit = (playerUnit) ? playerUnit : enemyUnit;
            var secondUnit = (playerUnit) ? enemyUnit : playerUnit;

            //creature of the second unit stored here
            var secondCreature = secondUnit.Creature;

            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Creature.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            //end the fight if creature KOd
            if (state == BattleState.BattleOverKO) yield break;

            //only handle second turn if second creature hasnt fainted
            if (secondCreature.HP > 0)
            {
                //second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Creature.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOverKO) yield break;
            }
        }
        else
        {
            //this is how the palyer switches creatures
            if (playerAction == BattleAction.SwitchCreature)
            {
                var selectedCreature = playerteam.Creatures[currentMember];
                yield return SwitchCreature(selectedCreature);
            }
            //attempt to cathch!!!
            else if(playerAction == BattleAction.UseItem)
            {
                dialogueBox.EnableActionText(false);
                yield return ThrowCaptureRing();
            }
            //attempt to run!
            else if(playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            //Enemy turn
            var enemyMove = enemyUnit.Creature.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOverKO) yield break;
        }

        if (state != BattleState.BattleOverKO)
        {
            ActionSelection();
        }
    }

    ///made a newer version that allowed for better flow
    ////this coroutine selects the currently placed move and returns feedback to the dialogue box
    //IEnumerator PlayerMove()
    //{
    //    state = BattleState.RunningTurn;

    //    var move = playerUnit.Creature.Moves[currentMove];
    //    yield return RunMove(playerUnit, enemyUnit, move);
    //    //large chunk of code removed and replaced with RunMove()

    //    //if the battle state was not changed by the RunMove then next step
    //    if (state == BattleState.RunningTurn)
    //    {
    //        StartCoroutine(EnemyMove());
    //    }
    //}

    //This allows for the enemy turn, same formula as the players after the choice. Move is chosen by random
    //IEnumerator EnemyMove()
    //{
    //    state = BattleState.RunningTurn;

    //    var move = enemyUnit.Creature.GetRandomMove();
    //    yield return RunMove(enemyUnit, playerUnit, move);

    //    //if the battle state was not changed by the RunMove then next step
    //    if (state == BattleState.RunningTurn)
    //    {
    //        ActionSelection();
    //    }
    //}

    //function created to condence similar funtionallity of player and enemy creatures
    //source unit is the one doing the move, the target is recieving it
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //checks to see if the cresture is effected by a modifier that would stop runmove
        bool canRunMove = sourceUnit.Creature.OnBeforeMove();
        if (!canRunMove)
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
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Creature, targetUnit.Creature, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Creature.TakeDamage(move, sourceUnit.Creature);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Creature.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.Creature, targetUnit.Creature, secondary.Target);
                    }
                }
            }

            //this coroutine plays if the creature loses all hp
            if (targetUnit.Creature.HP <= 0)
            {
                //replaced logic with
                yield return HandleCreatureFainted(targetUnit);
                //yield return dialogueBox.TypeDialog($"{targetUnit.Creature.Base.Name} fainted");
                //targetUnit.BattleFaintAnimation();
                //yield return new WaitForSeconds(2f);

                //CheckForBattleOver(targetUnit);
            }
        }
        //if the move misses
        else
        {
            yield return dialogueBox.TypeDialog($"{sourceUnit.Creature.Base.Name}'s attack missed");
        }
    }

    //moved this out of Runmove as there will hopefully be a lot of stats changes in here in the future
    IEnumerator RunMoveEffects(MoveEffects effects, Creature source, Creature target, MoveTarget moveTarget)
    {
        //var effects = move.Base.Effects;

        //stat boosting
        if (effects.Boosts != null)
        {
            //see if the move is targeting what creature, self boost or decline on enemy
            if (moveTarget == MoveTarget.self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        //check if this move will cause a status condition
        if (effects.Status != ConditionsID.none)
        {
            target.SetStatus(effects.Status);
        }

        //check if this move will cause a Volitilestatus condition
        if (effects.VolitileStatus != ConditionsID.none)
        {
            target.SetVolitileStatus(effects.VolitileStatus);
        }

        //now we apply the boost we show the status function to either
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        //end if the battle is over
        if (state == BattleState.BattleOverKO) yield break;
        //pass until this condition is true
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //status like poison and burn will hurt the creature after a turn
        sourceUnit.Creature.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Creature);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Creature.HP <= 0)
        {
            yield return HandleCreatureFainted(sourceUnit);
            //yield return dialogueBox.TypeDialog($"{sourceUnit.Creature.Base.Name} fainted");
            //sourceUnit.BattleFaintAnimation();
            //yield return new WaitForSeconds(2f);

            //CheckForBattleOver(sourceUnit);

            // SAM - is this neccissary? - SAM
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    //this will determine if a move will hit
    bool CheckIfMoveHits(Move move, Creature source, Creature target)
    {
        if (move.Base.CantMiss)
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
        if (accuracy > 0)
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }

        //evasion adjuster
        if (evasion > 0)
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
        while (creature.StatusChange.Count > 0)
        {
            var message = creature.StatusChange.Dequeue();
            yield return dialogueBox.TypeDialog(message);
        }
    }

    //this will now hold all fainted calculations
    IEnumerator HandleCreatureFainted(BattleUnit faintedUnit)
    {
        yield return dialogueBox.TypeDialog($"{faintedUnit.Creature.Base.Name} fainted");
        faintedUnit.BattleFaintAnimation();
        yield return new WaitForSeconds(2f);

        if(!faintedUnit.IsPlayerUnit)
        {
            //Experiance gain
            int expYield = faintedUnit.Creature.Base.ExpYield;
            int enemyLevel = faintedUnit.Creature.Level;
            //float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            //formula to calculate the experience gain
            int expGain = Mathf.FloorToInt((expYield * enemyLevel /** trainerBonus*/) / 7);
            //this is wehre the experience gain is added to our littel boy
            playerUnit.Creature.Exp += expGain;
            yield return dialogueBox.TypeDialog($"{playerUnit.Creature.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();

            //check level up


            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    //what happens if the target unit fainted
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
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
        if (damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialog("A critical hit!");
        }
        if (damageDetails.TypeEffectiveness > 1f)
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
        if (state == BattleState.ActionSelection)
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //0 = fight state
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //1 = bag state
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                //2 = creature state
                priorState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //3 = run state
                StartCoroutine(RunTurns(BattleAction.Run));
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //store the current move
            var move = playerUnit.Creature.Moves[currentMove];
            //if PP is 0 dont use move, return
            if (move.Pp == 0) return;

            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerteam.Creatures[currentMember];
            //checks if the selected creature has fainted
            if (selectedMember.HP <= 0)
            {
                battleTeamScreen.SetMessageText("You can't send out a fainted Creature");
                return;
            }
            if (selectedMember == playerUnit.Creature)
            {
                battleTeamScreen.SetMessageText("This creature is already out");
                return;
            }
            //disables select screen, sets to busy and start switch
            battleTeamScreen.gameObject.SetActive(false);

            //this is called if the plaer chooses to switch creature during their turn
            if (priorState == BattleState.ActionSelection)
            {
                priorState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchCreature));
            }
            else
            {
                //this is called if the creature fainted
                state = BattleState.Busy;
                StartCoroutine(SwitchCreature(selectedMember));
            }
        }
        //to back out press x
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleTeamScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    //used to switch creatures
    IEnumerator SwitchCreature(Creature newCreature)
    {
        //bool currentCreatureFainted = true;

        if (playerUnit.Creature.HP > 0)
        {
            //currentCreatureFainted = false;
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

        state = BattleState.RunningTurn;

        //if (currentCreatureFainted)
        //{
        //    ChooseFirstTurn();
        //}
        //else
        //{
        //    //players turn over, enemy turn
        //    StartCoroutine(EnemyMove());
        //}
    }

    IEnumerator ThrowCaptureRing()
    {
        state = BattleState.Busy;
        ////SAM - redo this, if we use trainer battles? - SAM
        //if(isTrainerBattle)
        //{
        //    yield return dialogueBox.TypeDialog($"You can't steal another teams Creatures!");
        //    state = BattleState.RunningTurn;
        //    yield break;
        //}


        //SAM - redo this message, figure out how to set up a players name? - SAM
        yield return dialogueBox.TypeDialog($"Player sent out a Capture Ring");

        //message pops up and we instantiate capture ring in
        var capturering = Instantiate(captureRing, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var CaptureRing = capturering.GetComponent<SpriteRenderer>();

        //implament the animations to throw the Capture Ring
        yield return CaptureRing.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 1), 0.5f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimaton();
        //drop Capture ring towards creature
        yield return CaptureRing.transform.DOMoveY(enemyUnit.transform.position.y - 2.1f, 0.5f).WaitForCompletion();

        //call the mathmatical equasion to determin if the creature is caught
        int shakeCount = TryToCatchCreature(enemyUnit.Creature);

        //animate the ring shake 3 times
        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return CaptureRing.transform.DOPunchRotation(new Vector3(0, 0, 8f), 0.8f).WaitForCompletion();
        }

        if(shakeCount == 4)
        {
            //creature was caught
            yield return dialogueBox.TypeDialog($"{enemyUnit.Creature.Base.Name} was caught");
            yield return CaptureRing.DOFade(0, 1.5f).WaitForCompletion();

            //add creature the player party
            playerteam.AddCreature(enemyUnit.Creature);
            yield return dialogueBox.TypeDialog($"{enemyUnit.Creature.Base.Name} has been added to your team");

            //remove capture ring
            Destroy(CaptureRing);
            BattleOverKO(true);
        }
        else
        {
            //creature escaped
            yield return new WaitForSeconds(0.5f);
            yield return CaptureRing.DOFade(0, 0.2f);
            //escape animation played
            yield return enemyUnit.PlayEscapeAnimaton();

            //two different dialoge box responsces depending on how close you were to catching the creature
            if(shakeCount < 2)
            {
                yield return dialogueBox.TypeDialog($"{enemyUnit.Creature.Base.Name} broke free");
            }
            else
            {
                yield return dialogueBox.TypeDialog($"Almost caught it");
            }

            //if the creature escapes the ring is destoryed and the battle continues
            Destroy(CaptureRing);
            state = BattleState.RunningTurn;
        }
    }

    //used in the age old method of enslaving creatures
    int TryToCatchCreature(Creature creature)
    {
        //the formula used in pokemon to capture using health, status and luck to try catch it
        float a = (3 * creature.MaxHp - 2 * creature.HP) * creature.Base.CatchRate * ConditionsDB.GetStatusBonus(creature.Status) / (3 * creature.MaxHp);
        if(a >= 255)
        {
            //on return 4 the creature is captured
            return 4;
        }

        //if the value is not equal to 255 a new value is required and its below....
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        //run loop 4 times
        while(shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }
            ++shakeCount;
        }
        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        //SAM - set this up if we decide to have trainer battles - SAM
        //if(isTrainerBattle)
        //{
        //    yield return dialogueBox.TypeDialog($"You can't run from a Trainer Battle");
        //    state = BattleState.RunningTurn;
        //    yield break;
        //}

        ++escapeAttempts;

        //check and store player and enemys speed
        int playerSpeed = playerUnit.Creature.Speed;
        int enemySpeed = enemyUnit.Creature.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogueBox.TypeDialog($"Ran away safely");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogueBox.TypeDialog($"Ran away safely");
                BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialog($"Can't escape!");
                state = BattleState.RunningTurn;
            }
        }

    }
}

