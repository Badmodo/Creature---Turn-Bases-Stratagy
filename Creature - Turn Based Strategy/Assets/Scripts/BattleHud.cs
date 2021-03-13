using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(Creature creature)
    {
        nameText.text = creature.Base.Name;
        levelText.text = "Lvl" + creature.Level;
        hpBar.SetHp((float)creature.HP / creature.MaxHp);
    }
}
