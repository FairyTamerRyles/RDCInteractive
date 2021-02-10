using UnityEngine;
using UnityEngine.UI;

public class Scaffle : MonoBehaviour {
    public ConnectionManager connectionManager;
    public MatchmakingManager matchmakingManager;
    public GameNetworkingManager gameNetworkingManager;
    public InputField roomNameField;
    public InputField messageField;

    public GameObject[] connectionCylinders;
    public GameObject[] roomCylinders;

    void Awake() {
        gameNetworkingManager.OnOpponentMoved_Callback = Receive;
    }

    public void Connect_Callback() {
        connectionCylinders[0].SetActive(false);
        connectionCylinders[1].SetActive(false);
        connectionCylinders[2].SetActive(true);
        Debug.Log("Connected");
    }

    public void Disconnect_Callback() {
        connectionCylinders[1].SetActive(false);
        connectionCylinders[2].SetActive(false);
        connectionCylinders[0].SetActive(true);
        Debug.Log("Disconnected");
    }

    public void CreatedRoom_Callback() {
        Debug.Log("Joined Room");
        roomNameField.text = matchmakingManager.RoomName;
    }

    public void JoinedRoom_Callback() {
        roomCylinders[0].SetActive(false);
        roomCylinders[1].SetActive(false);
        roomCylinders[2].SetActive(true);
        Debug.Log("Joined Room");
    }

    public void LeftRoom_Callback() {
        roomCylinders[1].SetActive(false);
        roomCylinders[2].SetActive(false);
        roomCylinders[0].SetActive(true);
        Debug.Log("Left Room");
    }

    public void Connect() {
        connectionCylinders[0].SetActive(false);
        connectionCylinders[2].SetActive(false);
        connectionCylinders[1].SetActive(true);
        connectionManager.Connect(Connect_Callback);
    }

    public void Disconnect() {
        connectionManager.Disconnect(Disconnect_Callback);
    }

    public void CreatePrivateRoom() {
        roomCylinders[0].SetActive(false);
        roomCylinders[2].SetActive(false);
        roomCylinders[1].SetActive(true);
        matchmakingManager.CreatePrivateRoom(CreatedRoom_Callback);
    }

    public void JoinRoom() {
        roomCylinders[0].SetActive(false);
        roomCylinders[2].SetActive(false);
        roomCylinders[1].SetActive(true);
        matchmakingManager.JoinRoom(roomNameField.text, JoinedRoom_Callback);
    }

    public void JoinRandomRoom() {
        roomCylinders[0].SetActive(false);
        roomCylinders[2].SetActive(false);
        roomCylinders[1].SetActive(true);
        matchmakingManager.JoinRandomRoom(JoinedRoom_Callback);
    }

    public void LeaveRoom() {
        matchmakingManager.LeaveRoom(LeftRoom_Callback);
    }

    public void Send() {
        gameNetworkingManager.Message =  messageField.text;
    }

    public void Receive() {
        messageField.text = gameNetworkingManager.Message;
    }
}
