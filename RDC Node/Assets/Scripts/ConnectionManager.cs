using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class ConnectionManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    private string gameVersion = "1";

    private Action failedToConnect_Callback;
    private Action onConnected_Callback;
    private Action onDisconnected_Callback;

    public Action FailedToConnect_Callback {
        get => failedToConnect_Callback;
        set => failedToConnect_Callback = value;
    }

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
            if(Application.internetReachability == NetworkReachability.NotReachable) {
                OnFailedToConnect();
            }
            else {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
    }

    public void Connect(Action onConnected_Callback) {
        OnConnected_Callback = onConnected_Callback;
        Connect();
    }

    public void Connect(Action onConnected_Callback, Action failedToConnect_Callback) {
        OnConnected_Callback = onConnected_Callback;
        FailedToConnect_Callback = failedToConnect_Callback;
        Connect();
    }

    public void Disconnect() {
        if (IsConnected()) {
            PhotonNetwork.Disconnect();
        }
    }

    public void Disconnect(Action onDisconnected_Callback) {
        OnDisconnected_Callback = onDisconnected_Callback;
        Disconnect();
    }

    public void OnFailedToConnect() {
        Action callback = FailedToConnect_Callback;
        if (callback != null) callback();
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
