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

    public void heelClick()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("HeelStep");
    }

    public void hmmm()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("Hmmm");
    }

    public void light()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("LightSwitch");
    }

    public void fight()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("InternLose");
        soundController.GetComponent<SoundManager>().PlaySFX("ScientistAction2");
    }

    public void bubbly()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("AlchemyApparatus");
    }

    public void heh()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("InternTurn1");
    }

    public void verses()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("VsSound");
    }

    public void playClick()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("MarkerCap");
    }
    public void playThud()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("MarkerOnDryErase");
    }

    public void playTech()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("CoverOpen");
    }



    void Start()
    {
        soundController = GameObject.Find("SoundManager");
    }
}
