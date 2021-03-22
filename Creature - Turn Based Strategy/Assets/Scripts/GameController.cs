using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this state controller designates what happens in what state freeroam or while in battle
public enum GameState { Freeroam, Battle, Dialogue }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController3D playerController3D;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera FreeroamCam;

    [SerializeField] GameObject Player;

    static GameState state;
    public static GameController Instance;
    public static GameState State => state;

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Initilize();
    }

    private void Start()
    {
        playerController3D.onEncounter += StartBattle;
        battleSystem.BattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.Dialogue;
        };
        
        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.Dialogue)
            {
                state = GameState.Freeroam;
            }
        };
    }

    //start battle state
    void StartBattle()
    {
        if (state != GameState.Battle) //Preventing from entering a battle when already in one. 
        {
            Player.SetActive(false);

            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            FreeroamCam.gameObject.SetActive(false);

            //used to return the player creatures
            var playerTeam = playerController3D.GetComponent<CreatureTeam>();
            var wildCreature = FindObjectOfType<ListOfCreaturesInArea>().GetComponent<ListOfCreaturesInArea>().GetRandomWildCreatures();

            //bug that the captured creature would not show up because it was in your team, in the grass... Odd
            var wildCreatureCopy = new Creature(wildCreature.Base, wildCreature.Level);
            battleSystem.StartBattle(playerTeam, wildCreatureCopy);
        }
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        if (state != GameState.Battle) //Preventing from entering a battle when already in one. 
        {
            //Player.SetActive(false);

            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            FreeroamCam.gameObject.SetActive(false);

            //used to return the player creatures
            var playerTeam = playerController3D.GetComponent<CreatureTeam>();
            var trainerTeam = trainer.GetComponent<CreatureTeam>();

            battleSystem.StartTrainerBattle(playerTeam, trainerTeam);
        }
    }


    //start freeroam state
    void EndBattle(bool won)
    {

        //Player.SetActive(true);

        state = GameState.Freeroam;
        battleSystem.gameObject.SetActive(false);
        FreeroamCam.gameObject.SetActive(true);

    }

    void Update()
    {
        if( state == GameState.Freeroam)
        {
            playerController3D.HandleUpdate();
        }
        else if( state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if( state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }
}
