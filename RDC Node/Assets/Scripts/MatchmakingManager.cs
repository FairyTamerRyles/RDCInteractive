using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MatchmakingManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private static byte maxPlayersPerRoom = 2;

    private bool firstInRoom = false;
    private int hostPlayer;
    
    public bool FirstInRoom {
        get => firstInRoom;
        set => firstInRoom = value;
    }

    public int HostPlayer {
        get => hostPlayer;
        set {
            hostPlayer = value;
            if (RoomName != null) {
                PhotonView.Get(this).RPC("SetHostPlayer", RpcTarget.Others, value);
            }
        }
    }
    
    private bool creatingPrivateRoom = false;
    private int createRoomMaxAttempts = 50;
    private int createRoomAttempts = 0;

    private Action onJoinedRoom_Callback;
    private Action onLeftRoom_Callback;
    private Action onJoinRandomFailed_Callback;
    private Action onCreatePrivateRoomFailed_Callback;
    private Action onJoinRoomFailed_Callback;
    private Action onHostSet_Callback;

    public Action OnJoinedRoom_Callback {
        get => onJoinedRoom_Callback;
        set => onJoinedRoom_Callback = value;
    }

    public Action OnLeftRoom_Callback {
        get => onLeftRoom_Callback;
        set => onLeftRoom_Callback = value;
    }

    public Action OnJoinRandomFailed_Callback {
        get => onJoinRandomFailed_Callback;
        set => onJoinRandomFailed_Callback = value;
    }

    public Action OnCreatePrivateRoomFailed_Callback {
        get => onCreatePrivateRoomFailed_Callback;
        set => onCreatePrivateRoomFailed_Callback = value;
    }

    public Action OnJoinRoomFailed_Callback {
        get => onJoinRoomFailed_Callback;
        set => onJoinRoomFailed_Callback = value;
    }

    public Action OnHostSet_Callback {
        get => onHostSet_Callback;
        set => onHostSet_Callback = value;
    }

    public String RoomName {
        get {
            return (PhotonNetwork.CurrentRoom == null) ? "" : PhotonNetwork.CurrentRoom.Name;
        }
    }

    public bool InRoom {
        get {
            return PhotonNetwork.CurrentRoom != null;
        }
    }

    public void CreatePrivateRoom() {
        if (ConnectionManager.IsConnected()) {
            creatingPrivateRoom = true;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayersPerRoom;
            roomOptions.IsVisible = false;

            string roomName = RoomNameGenerator.Next();

            FirstInRoom = true;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void CreatePrivateRoom(Action onJoinedRoom_Callback) {
        OnJoinedRoom_Callback = onJoinedRoom_Callback;
        CreatePrivateRoom();
    }

    public void CreatePrivateRoom(Action onJoinedRoom_Callback, Action onCreatePrivateRoomFailed_Callback) {
        OnCreatePrivateRoomFailed_Callback = onCreatePrivateRoomFailed_Callback;
        CreatePrivateRoom(onJoinedRoom_Callback);
    }

    public void JoinRoom(string roomName) {
        if (ConnectionManager.IsConnected()) {
            FirstInRoom = false;
            PhotonNetwork.JoinRoom(roomName.ToUpper());
        }
    }

    public void JoinRoom(string roomName, Action onJoinedRoom_Callback) {
        OnJoinedRoom_Callback = onJoinedRoom_Callback;
        JoinRoom(roomName);
    }

    public void JoinRoom(string roomName, Action onJoinedRoom_Callback, Action onJoinRoomFailed_Callback) {
        OnJoinRoomFailed_Callback = onJoinRoomFailed_Callback;
        JoinRoom(roomName, onJoinedRoom_Callback);
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

    public void JoinRandomRoom(Action onJoinedRoom_Callback, Action onJoinRandomFailed_Callback) {
        OnJoinRandomFailed_Callback = onJoinRandomFailed_Callback;
        JoinRandomRoom(onJoinedRoom_Callback);
    }

    public static void LeaveRoom() {
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
        FirstInRoom = true;
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom() {
        creatingPrivateRoom = false;
        createRoomAttempts = 0;

        Action callback = OnJoinedRoom_Callback;
        if (callback != null) callback();
    }

    public override void OnLeftRoom() {
        Action callback = OnLeftRoom_Callback;
        if (callback != null) callback();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        if (creatingPrivateRoom && ++createRoomAttempts < createRoomMaxAttempts) {
            CreatePrivateRoom();
        } else if (creatingPrivateRoom) {
            Action callback = OnCreatePrivateRoomFailed_Callback;
            ConnectionManager.Disconnect();
            if (callback != null) callback();
        } else {
            Action callback = OnJoinRandomFailed_Callback;
            if (callback != null) callback();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Action callback = onJoinRoomFailed_Callback;
        if (callback != null) callback();
    }

    public override void OnConnectedToMaster() {
        createRoomAttempts = 0;
    }

    [PunRPC]
    private void SetHostPlayer(int hostPlayer) {
        HostPlayer = hostPlayer;

        Action callback = OnHostSet_Callback;
        if (callback != null) callback();
    }
}
