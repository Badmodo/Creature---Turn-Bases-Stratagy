using DG.Tweening;
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
    [SerializeField] GameObject expBar;

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
        SetLevel();
        hpBar.SetHp((float)creature.HP / creature.MaxHp);
        SetExp();

        //setting the status colours in the Hud through disctionary
        statusColors = new Dictionary<ConditionsID, Color>()
        {
            {ConditionsID.psn, psnColor },
            {ConditionsID.brn, brnColor },
            {ConditionsID.slp, slpColor },
            {ConditionsID.par, parColor },
            {ConditionsID.frz, frzColor },
        };

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
            statusText.color = statusColors[_creature.Status.Id];
        }
    }

    //this will set the level in the Hud
    public void SetLevel()
    {
        levelText.text = "Lvl" + _creature.Level;
    }

    //sets the value on the xp bar
    public void SetExp()
    {
        //only the player bar will have an XP Bar
        if (expBar == null) return;

        float normalisedExp = GetNormalisedXP();
        expBar.transform.localScale = new Vector3(normalisedExp, 1, 1);
    }
    
    //set exp smoothly into ExpBar
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        //only the player bar will have an XP Bar
        if (expBar == null) yield break;

        if(reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalisedExp = GetNormalisedXP();
        yield return expBar.transform.DOScaleX(normalisedExp, 1.5f).WaitForCompletion();
    }

    //to normalise the exp we need the xp required for that level
    float GetNormalisedXP()
    {
        int currentLevelExp = _creature.Base.GetExpForLevel(_creature.Level);
        int nextLevelExp = _creature.Base.GetExpForLevel(_creature.Level + 1);

        float normalizesXP = (float)(_creature.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizesXP);
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
