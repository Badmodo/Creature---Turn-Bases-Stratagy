using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Creature _creature;

    Dictionary<ConditionsID, Color> statusColors; 

    public void SetData(Creature creature)
    {
        _creature = creature;

        nameText.text = creature.Base.Name;
        levelText.text = "Lvl" + creature.Level;
        hpBar.SetHp((float)creature.HP / creature.MaxHp);

        SetStatusText();
        //whenever the status is changed this function will be called
        _creature.OnStatusChanged += SetStatusText;
    }

    //this allows the status to be visualised in the HUD
    void SetStatusText()
    {
        if(_creature.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _creature.Status.Id.ToString().ToUpper();
        }
    }

    //updates the Hp loss with a smooth transition
    public IEnumerator UpdateHP()
    {
        if (_creature.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_creature.HP / _creature.MaxHp);
            _creature.HpChanged = false;
        }
    }
}
