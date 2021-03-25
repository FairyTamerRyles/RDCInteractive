using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("humanPlayer", 1);
    }

    public void AIGameSelected()
    {
        PlayerPrefs.SetString("gameType", "AI");
    }

    public void NetworkGameSelected()
    {
        PlayerPrefs.SetString("gameType", "Network");
    }

    public void SetPlayer()
    {
        if(GameObject.FindGameObjectWithTag("P1Toggle").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("humanPlayer", 1);
        }
        else
        {
            PlayerPrefs.SetInt("humanPlayer", 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
