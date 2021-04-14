using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsController : MonoBehaviour
{
    public GameObject bst;
    public GameObject self;
    public GameObject joinPrivate;

    public GameObject connectionErrorBox;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(GameObject.Find("NetworkingObjects"));

        DontDestroyOnLoad(GameObject.Find("SoundManager"));

        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();

        connectionManager.GetComponent<ConnectionManager>().OnDisconnected_Callback = () => {ConnectionError();};

        GameObject.Find("SoundManager").GetComponent<SoundManager>().Play("BamGoozledMenu");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) == true && Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow) && self.name == "SettingsController")
        {
            bst.SetActive(!bst.activeSelf);
        }
    }

    public void KillTheFade()
    {
        Destroy(GameObject.Find("Fader"));
    }

    public void AIGameSelected()
    {
        PlayerPrefs.SetString("gameType", "AI");
        SetAIDifficulty("Easy");
    }

    public void SetAIDifficulty(string buttonPressed)
    {
        if(buttonPressed == "Hard")
        {
            Debug.Log("Hard game picked");
            PlayerPrefs.SetString("difficulty", "Hard");
        }
        else
        {
            Debug.Log("Easy game picked");
            PlayerPrefs.SetString("difficulty", "Easy");
        }
    }

    public void AIDifficultyButtonClicked()
    {
        string buttonPressed = EventSystem.current.currentSelectedGameObject.name;
        SetAIDifficulty(buttonPressed);
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
            GameObject.FindGameObjectWithTag("P1ToggleAI").GetComponent<Toggle>().interactable = false;
            GameObject.FindGameObjectWithTag("P2ToggleAI").GetComponent<Toggle>().interactable = true;
        }
        else
        {
            PlayerPrefs.SetInt("humanPlayer", 2);
            GameObject.FindGameObjectWithTag("P2ToggleAI").GetComponent<Toggle>().interactable = false;
            GameObject.FindGameObjectWithTag("P1ToggleAI").GetComponent<Toggle>().interactable = true;
        }
    }

    public void SetPlayerForPrivateNetworkGame()
    {
        if(GameObject.FindGameObjectWithTag("P1Toggle").GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("humanPlayer", 1);
            GameObject.FindGameObjectWithTag("P1Toggle").GetComponent<Toggle>().interactable = false;
            GameObject.FindGameObjectWithTag("P2Toggle").GetComponent<Toggle>().interactable = true;
        }
        else
        {
            PlayerPrefs.SetInt("humanPlayer", 2);
            GameObject.FindGameObjectWithTag("P1Toggle").GetComponent<Toggle>().interactable = true;
            GameObject.FindGameObjectWithTag("P2Toggle").GetComponent<Toggle>().interactable = false;
        }
    }

    public void JoinRandomGame()
    {
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();
        var gameNetworkingManager = GameObject.Find("GameNetworkingManager").GetComponent<GameNetworkingManager>();
        connectionManager.Connect(() => {
            Debug.Log("Connected");
            matchmakingManager.JoinRandomRoom(() => {
                Debug.Log("In Room");
                if(matchmakingManager.FirstInRoom)
                {
                    PlayerPrefs.SetInt("humanPlayer", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("humanPlayer", 2);
                }
                gameNetworkingManager.OnRoomFull_Callback = () => {
                    GameObject.Find("Fader").GetComponent<Animator>().SetBool("LeavingScene", true);
                };
            }, () => {ConnectionError();});
        }, () => {ConnectionError();});
    }

    public void BeginAIGame()
    {
        SetPlayerForAIGame();
        if(bst.GetComponent<InputField>().text != "")
        {
            PlayerPrefs.SetString("boardSeed", bst.GetComponent<InputField>().text);
        }
        GameObject.Find("Fader").GetComponent<Animator>().SetBool("LeavingScene", true);
    }

    public void CreatePrivateRoom()
    {
        Debug.Log("dresting private room");
        CleanRoomCodeBox();
        SetPlayerForPrivateNetworkGame();
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();
        var gameNetworkingManager = GameObject.Find("GameNetworkingManager").GetComponent<GameNetworkingManager>();

        matchmakingManager.OnCreatePrivateRoomFailed_Callback = () => {Debug.Log("Creation of room failed");};

        connectionManager.Connect(() => {
            Debug.Log("Connected");
            matchmakingManager.CreatePrivateRoom(() => {
                Debug.Log("Joined Room");
                GameObject.Find("Room Code").GetComponent<Text>().text = matchmakingManager.RoomName;
                gameNetworkingManager.OnRoomFull_Callback = () => {
                    matchmakingManager.HostPlayer = PlayerPrefs.GetInt("humanPlayer");
                    GameObject.Find("Fader").GetComponent<Animator>().SetBool("LeavingScene", true);
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
                    GameObject.Find("Fader").GetComponent<Animator>().SetBool("LeavingScene", true);
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
        Debug.Log("networking game cancelled");
        var connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        var matchmakingManager = GameObject.Find("MatchmakingManager").GetComponent<MatchmakingManager>();

        if(matchmakingManager.InRoom)
        {
            matchmakingManager.LeaveRoom(() =>{
                Debug.Log("Left Room");
                connectionManager.Disconnect(() =>{
                    Debug.Log("disconnected");
                });
            });
        }
        else if(ConnectionManager.IsConnected())
        {
            connectionManager.Disconnect(() =>{
                Debug.Log("disconnected");
            });
        }
            
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

    public void roomToJoinValueChanged()
    {
        var typeRoom = GameObject.Find("Type Room");
        if(typeRoom)
        {
            var roomToJoinBox = GameObject.Find("Type Room").GetComponent<InputField>();
            roomToJoinBox.text = roomToJoinBox.text.ToUpper();
            Debug.Log(roomToJoinBox.text);
            if(roomToJoinBox.text.Length == 5)
            {
                joinPrivate.SetActive(true);
            }
            else
            {
                joinPrivate.SetActive(false);
            }
        }
    }

    public void goToLoadingScreen()
    {
        GameObject.FindGameObjectWithTag("ChangeScene").GetComponent<ChangeScene>().loadlevel("LoadingScreen");
        GameObject.Find("SoundManager").GetComponent<SoundManager>().TransitionToSilence();
    }
}
