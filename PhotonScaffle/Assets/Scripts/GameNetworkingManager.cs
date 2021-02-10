using UnityEngine;
using Photon.Pun;

public class GameNetworkingManager : MonoBehaviour {
    private string message = "";

    public string Message {
        get => message;
        set => PhotonView.Get(this).RPC("MakeMove", RpcTarget.All, value);
    }

    [PunRPC]
    void MakeMove(string message) {
        this.message = message;
    }
}
