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
        int rSound = (int)(Floor(Random.Range(1.0f, 3.0f)));
            soundController.GetComponent<SoundManager>().PlaySFX("InternTurn" + rSound);
    }

    void Start()
    {
        soundController = GameObject.Find("SoundManager");
    }
}
