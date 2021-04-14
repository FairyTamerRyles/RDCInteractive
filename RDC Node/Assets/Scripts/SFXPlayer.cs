using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class SFXPlayer : MonoBehaviour
{
    public GameObject soundController;

    public void vatClose()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("CoverOpen");
    }

    public void internEnter()
    {
        int rSound = (int)(Floor(Random.Range(1.0f, 3.5f)));
            soundController.GetComponent<SoundManager>().PlaySFX("InternTurn" + rSound);
    }
    
    public void scientistEnter()
    {
        int rSound = (int)(Floor(Random.Range(1.0f, 2.5f)));
            soundController.GetComponent<SoundManager>().PlaySFX("ScientistTurnStart" + rSound);
    }

    void Start()
    {
        soundController = GameObject.Find("SoundManager");
    }
}
