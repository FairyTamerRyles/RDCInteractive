using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ConnectionManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private string gameVersion = "1";

    private Action onConnected_Callback;
    private Action onDisconnected_Callback;

    public Action OnConnected_Callback {
        get => onConnected_Callback;
        set => onConnected_Callback = value;
    }

    public Action OnDisconnected_Callback {
        get => onDisconnected_Callback;
        set => onDisconnected_Callback = value;
    }

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect() {
        if (!IsConnected()) {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void Connect(Action callback) {
        OnConnected_Callback = callback;
        Connect();
    }

    public void Disconnect() {
        if (IsConnected()) {
            PhotonNetwork.Disconnect();
        }
    }

    public void Disconnect(Action callback) {
        OnDisconnected_Callback = callback;
        Disconnect();
    }

    public override void OnConnectedToMaster() {
        Action callback = OnConnected_Callback;
        if (callback != null) callback();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Action callback = OnDisconnected_Callback;
        if (callback != null) callback();
    }

    public static bool IsConnected() {
        return PhotonNetwork.IsConnected;
    }
}
