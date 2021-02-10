using UnityEngine;
using Photon.Pun;
using System;

public class GameNetworkingManager : MonoBehaviour {
    private string message = "";

    private Action onOpponentMoved_Callback;

    public string Message {
        get => message;
        set => PhotonView.Get(this).RPC("MakeMove", RpcTarget.All, value);
    }

    public Action OnOpponentMoved_Callback {
        get => onOpponentMoved_Callback;
        set => onOpponentMoved_Callback = value;
    }

    [PunRPC]
    private void MakeMove(string message) {
        this.message = message;
        OnOpponentMoved();
    }

    public void OnOpponentMoved() {
        Action callback = OnOpponentMoved_Callback;
        if (callback != null) callback();
    }
}
