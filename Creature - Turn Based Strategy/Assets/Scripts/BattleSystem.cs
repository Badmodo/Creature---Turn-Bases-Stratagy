using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;

    private void Start()
    {
        SetUpBattle();
    }

    //simp;le set up to show player and enemy set up
    public void SetUpBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Creature);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Creature);
    }

}
