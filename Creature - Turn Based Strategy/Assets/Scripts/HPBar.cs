using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{

    [SerializeField] GameObject health;

    //private void Start()
    //{
    //    //testing if the HP bar works
    //    health.transform.localScale = new Vector3(0.5f, 1f);
    //}

    public void SetHp(float hpNormalised)
    {
        health.transform.localScale = new Vector3(hpNormalised, 1f);
    }
}
