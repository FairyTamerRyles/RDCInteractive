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
        DontDestroyOnLoad(GameObject.Find("NetworkingObjects"));
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

    public void JoinRandomGame()
    {
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();
        var gameNetworkingManager = GameObject.Find("GameNetworkingManager").GetComponent<GameNetworkingManager>();
        connectionManager.Connect(() => {
            matchmakingManager.JoinRandomRoom(() => {
                gameNetworkingManager.OnRoomFull_Callback = () => {GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");};
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
