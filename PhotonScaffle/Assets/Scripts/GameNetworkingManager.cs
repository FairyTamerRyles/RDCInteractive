using UnityEngine;
using Photon.Pun;

public class GameNetworkingManager : MonoBehaviourPunCallbacks, IPunObservable {
    private string message = "";

    public string Message {
        get => message;
        set => message = value;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(Message);
        } else {
            Message = (string) stream.ReceiveNext();
        }
    }
}
