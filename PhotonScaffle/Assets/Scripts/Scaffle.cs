using UnityEngine;
using UnityEngine.UI;

public class Scaffle : MonoBehaviour {
    public ConnectionManager connectionManager;
    public MatchmakingManager matchmakingManager;
    public GameNetworkingManager gameNetworkingManager;
    public InputField roomNameField;
    public InputField messageField;

    void Awake() {
        gameNetworkingManager.OnOpponentMoved_Callback = Receive;
    }

    public void Connect_Callback() {
        Debug.Log("Connected");
    }

    public void Disconnect_Callback() {
        Debug.Log("Disconnected");
    }

    public void CreatedRoom_Callback() {
        Debug.Log("Joined Room");
        roomNameField.text = matchmakingManager.RoomName;
    }

    public void JoinedRoom_Callback() {
        Debug.Log("Joined Room");
    }

    public void LeftRoom_Callback() {
        Debug.Log("Left Room");
    }

    public void Connect() {
        connectionManager.Connect(Connect_Callback);
    }

    public void Disconnect() {
        connectionManager.Disconnect(Disconnect_Callback);
    }

    public void CreatePrivateRoom() {
        matchmakingManager.CreatePrivateRoom(CreatedRoom_Callback);
    }

    public void JoinRoom() {
        matchmakingManager.JoinRoom(roomNameField.text, JoinedRoom_Callback);
    }

    public void JoinRandomRoom() {
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
