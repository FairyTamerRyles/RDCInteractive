using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour
{
    public GameObject connectionErrorBox;
    // Start is called before the first frame update
    void Start()
    {
        if(ifSceneCurrentlyLoaded("Game"))
        {
            SceneManager.UnloadScene("Game");
        }
        DontDestroyOnLoad(GameObject.Find("NetworkingObjects"));
        DontDestroyOnLoad(GameObject.Find("SoundManager"));

        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        connectionManager.GetComponent<ConnectionManager>().OnDisconnected_Callback = () => {ConnectionError();};
    }

    public void AIGameSelected()
    {
        PlayerPrefs.SetString("gameType", "AI");
    }

    public void NetworkGameSelected()
    {
        PlayerPrefs.SetString("gameType", "Network");
    }

    public void SetPlayerForAIGame()
    {
        if(GameObject.FindGameObjectWithTag("P1ToggleAI").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("humanPlayer", 1);
        }
        else
        {
            PlayerPrefs.SetInt("humanPlayer", 2);
        }
    }

    public void SetPlayerForPrivateNetworkGame()
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
                if(matchmakingManager.FirstInRoom)
                {
                    PlayerPrefs.SetInt("humanPlayer", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("humanPlayer", 2);
                }
                gameNetworkingManager.OnRoomFull_Callback = () => {
                    GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
                };
            }, () => {ConnectionError();});
        }, () => {ConnectionError();});
    }

    public void BeginAIGame()
    {
        SetPlayerForAIGame();
        GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
    }

    public void CreatePrivateRoom()
    {
        CleanRoomCodeBox();
        SetPlayerForPrivateNetworkGame();
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();
        var gameNetworkingManager = GameObject.Find("GameNetworkingManager").GetComponent<GameNetworkingManager>();

        connectionManager.Connect(() => {
            matchmakingManager.CreatePrivateRoom(() => {
                GameObject.Find("Room Code").GetComponent<Text>().text = matchmakingManager.RoomName;
                gameNetworkingManager.OnRoomFull_Callback = () => {
                    matchmakingManager.HostPlayer = PlayerPrefs.GetInt("humanPlayer");
                    GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
                };
            }, () => {ConnectionError();});
        }, () => {ConnectionError();});
    }

    public void JoinPrivateRoom()
    {
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();
        var gameNetworkingManager = GameObject.Find("GameNetworkingManager").GetComponent<GameNetworkingManager>();

        string roomName = GameObject.Find("RoomNameToJoin").GetComponent<Text>().text;

        connectionManager.Connect(() => {
            matchmakingManager.OnHostSet_Callback = (() => {PlayerPrefs.SetInt("humanPlayer", ((matchmakingManager.HostPlayer == 1) ? 2 : 1));});
            matchmakingManager.JoinRoom(roomName, () => {
                gameNetworkingManager.OnRoomFull_Callback = () => {
                    GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("Game");
                };
            },
            () => {ConnectionError();});
        }, () => {ConnectionError();});
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void cancelNetworkGame()
    {
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();

        matchmakingManager.LeaveRoom(() =>{
            connectionManager.Disconnect(() =>{
            });
        });
    }

    public void ConnectionError()
    {
        connectionErrorBox.SetActive(true);
    }

    public void CleanRoomCodeBox()
    {
        GameObject.Find("Room Code").GetComponent<Text>().text = "Loading...";
    }

    public void CleanRoomToJoinBox()
    {
        GameObject.Find("RoomNameToJoin").GetComponent<Text>().text = "";
    }

    bool ifSceneCurrentlyLoaded(string sceneName_no_extention) {
          for(int i = 0; i<SceneManager.sceneCount; ++i) {
             Scene scene = SceneManager.GetSceneAt(i);
             if(scene.name == sceneName_no_extention) {
                 //the scene is already loaded
                 return true;
             }
         }
 
          return false;//scene not currently loaded in the hierarchy
     }
}
