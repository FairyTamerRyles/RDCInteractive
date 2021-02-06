using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MatchmakingManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private static byte maxPlayersPerRoom = 2;
    
    private bool creatingPrivateRoom = false;

    private Action onJoinedRoom_Callback;
    private Action onLeftRoom_Callback;

    public Action OnJoinedRoom_Callback {
        get => onJoinedRoom_Callback;
        set => onJoinedRoom_Callback = value;
    }

    public Action OnLeftRoom_Callback {
        get => onLeftRoom_Callback;
        set => onLeftRoom_Callback = value;
    }

    public String RoomName {
        get {
            return (PhotonNetwork.CurrentRoom == null) ? "" : PhotonNetwork.CurrentRoom.Name;
        }
    }

    public void CreatePrivateRoom() {
        if (ConnectionManager.IsConnected()) {
            creatingPrivateRoom = true;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.IsVisible = false;

            string roomName = RoomNameGenerator.Next();

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void CreatePrivateRoom(Action onJoinedRoom_Callback) {
        OnJoinedRoom_Callback = onJoinedRoom_Callback;
        CreatePrivateRoom();
    }

    public void JoinRoom(string roomName) {
        if (ConnectionManager.IsConnected()) {
            PhotonNetwork.JoinRoom(roomName.ToUpper());
        }
    }

    public void JoinRoom(string roomName, Action onJoinedRoom_Callback) {
        OnJoinedRoom_Callback = onJoinedRoom_Callback;
        JoinRoom(roomName);
    }

    public void JoinRandomRoom() {
        if (ConnectionManager.IsConnected()) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void JoinRandomRoom(Action onJoinedRoom_Callback) {
        OnJoinedRoom_Callback = onJoinedRoom_Callback;
        JoinRandomRoom();
    }

    public void LeaveRoom() {
        if (ConnectionManager.IsConnected() && PhotonNetwork.CurrentRoom != null) {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void LeaveRoom(Action onLeftRoom_Callback) {
        OnLeftRoom_Callback = onLeftRoom_Callback;
        LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        CreateRoom();
    }

    private void CreateRoom() {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom() {
        creatingPrivateRoom = false;

        Action callback = OnJoinedRoom_Callback;
        if (callback != null) callback();
    }

    public override void OnLeftRoom() {
        Action callback = OnLeftRoom_Callback;
        if (callback != null) callback();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        if (creatingPrivateRoom) {
            CreatePrivateRoom();
        }
    }
}
