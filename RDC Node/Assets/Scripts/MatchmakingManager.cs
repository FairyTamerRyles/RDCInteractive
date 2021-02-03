using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MatchmakingManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private static byte maxPlayersPerRoom = 2;

    private Action onJoinedRoom_Callback;

    public Action OnJoinedRoom_Callback {
        get => onJoinedRoom_Callback;
        set => onJoinedRoom_Callback = value;
    }

    public String RoomName {
        get {
            return (PhotonNetwork.CurrentRoom == null) ? "" : PhotonNetwork.CurrentRoom.Name;
        }
    }

    public void CreatePrivateRoom() {
        if (ConnectionManager.IsConnected()) {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.IsVisible = false;

            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    public void CreatePrivateRoom(Action callback) {
        OnJoinedRoom_Callback = callback;
        CreatePrivateRoom();
    }

    public void JoinRoom(string roomName) {
        if (ConnectionManager.IsConnected()) {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void JoinRoom(string roomName, Action callback) {
        OnJoinedRoom_Callback = callback;
        JoinRoom(roomName);
    }

    public void JoinRandomRoom() {
        if (ConnectionManager.IsConnected()) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void JoinRandomRoom(Action callback) {
        OnJoinedRoom_Callback = callback;
        JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        CreateRoom();
    }

    private void CreateRoom() {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom() {
        Action callback = OnJoinedRoom_Callback;
        if (callback != null) callback();
    }
}
