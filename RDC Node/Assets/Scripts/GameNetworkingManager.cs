using UnityEngine;
using Photon.Pun;
using System;
using System.Timers;

public class GameNetworkingManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private static int maxPlayersPerRoom = 2;

    private int recordedPlayers = 0;
    private bool roomFull = false;
    private Timer queryForPlayersLeavingTimer;

    private bool RoomFull {
        get => roomFull;
        set {
            if (!roomFull && value) OnRoomFull();

            roomFull = value;
        }
    }

    private GameBoard board;

    private Action onOpponentMoved_Callback;
    private Action onRoomFull_Callback;

    public GameBoard Board {
        get => board;
        set {
            board = value;
            PhotonView.Get(this).RPC("MakeMove", RpcTarget.Others, value);
        }
    }

    public Action OnOpponentMoved_Callback {
        get => onOpponentMoved_Callback;
        set => onOpponentMoved_Callback = value;
    }

    public Action OnRoomFull_Callback {
        get => onRoomFull_Callback;
        set => onRoomFull_Callback = value;
    }

    [PunRPC]
    private void MakeMove(GameBoard board) {
        this.board = board;
        OnOpponentMoved();
    }

    public void OnOpponentMoved() {
        Action callback = OnOpponentMoved_Callback;
        if (callback != null) callback();
    }

    public override void OnJoinedRoom() {
        recordedPlayers = 0;
        roomFull = false;
        PhotonView.Get(this).RPC("IncrementRecordedPlayers", RpcTarget.All);
    }

    [PunRPC]
    private void IncrementRecordedPlayers() {
        if (roomFull) PhotonView.Get(this).RPC("DesignateRoomAsFull", RpcTarget.All);

        ++recordedPlayers;
        if (recordedPlayers == maxPlayersPerRoom) PhotonView.Get(this).RPC("DesignateRoomAsFull", RpcTarget.All);
    }

    [PunRPC]
    private void DesignateRoomAsFull() {
        recordedPlayers = 0;
        RoomFull = true;
    }

    public void OnRoomFull() {
        Action callback = OnRoomFull_Callback;
        if (callback != null) callback();
    }

    void Awake() {
        queryForPlayersLeavingTimer = new Timer(5000);
        queryForPlayersLeavingTimer.Elapsed += LeaveRoomIfPlayerLeft;
        queryForPlayersLeavingTimer.AutoReset = true;
        queryForPlayersLeavingTimer.Enabled = true;
    }

    void LeaveRoomIfPlayerLeft(object source, ElapsedEventArgs e) {
        if (RoomFull && PhotonNetwork.CurrentRoom?.PlayerCount < maxPlayersPerRoom) {
            MatchmakingManager.LeaveRoom();
        }
    }
}
