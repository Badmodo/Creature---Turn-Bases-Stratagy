using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this state controller designates what happens in what state freeroam or while in battle
public enum GameState { freeroam, Battle }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController3D playerController3D;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera FreeroamCam;

    [SerializeField] GameObject Player;

    GameState state;

    private void Start()
    {
        playerController3D.onEncounter += StartBattle;
        battleSystem.BattleOver += EndBattle;
    }

    //start battle state
    void StartBattle()
    {
        Player.SetActive(false);

        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        FreeroamCam.gameObject.SetActive(false);

        battleSystem.StartBattle();
    }


    //start freeroam state
    void EndBattle(bool won)
    {

        Player.SetActive(true);

        state = GameState.freeroam;
        battleSystem.gameObject.SetActive(false);
        FreeroamCam.gameObject.SetActive(true);

    }

    void Update()
    {
        if( state == GameState.freeroam)
        {
            playerController3D.HandleUpdate();
        }
        else if( state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
