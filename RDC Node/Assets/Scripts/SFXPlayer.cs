using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public GameObject soundController;

    public void vatClose()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("VatClose");
    }

    void Start()
    {
        soundController = GameObject.Find("SoundManager");
    }
}
