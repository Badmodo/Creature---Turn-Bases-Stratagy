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

    Creature creature;

    Dictionary<ConditionsID, Color> statusColors; 

    public void SetData(Creature creature)
    {
        this.creature = creature;

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
        this.creature.OnStatusChanged += SetStatusText;
    }

    //this allows the status to be visualised in the HUD
    void SetStatusText()
    {
        if(creature.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = creature.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[creature.Status.Id];
        }
    }

    //this will set the level in the Hud
    public void SetLevel()
    {
        levelText.text = "Lvl" + creature.Level;
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
        int currentLevelExp = creature.Base.GetExpForLevel(creature.Level);
        int nextLevelExp = creature.Base.GetExpForLevel(creature.Level + 1);

        float normalizesXP = (float)(creature.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizesXP);
    }


    //updates the Hp loss with a smooth transition
    public IEnumerator UpdateHP()
    {
        if (creature.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)creature.HP / creature.MaxHp);
            creature.HpChanged = false;
        }
    }
}
