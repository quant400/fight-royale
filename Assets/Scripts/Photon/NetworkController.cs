using UnityEngine;
using Photon.Pun;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playButton; 

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("PhotonNetwork.ConnectUsingSettings() -> PhotonNetwork is connected: " + PhotonNetwork.IsConnected);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are connected to " + PhotonNetwork.CloudRegion + " server");
        PhotonNetwork.AutomaticallySyncScene = true;
        playButton.SetActive(true);
        //PhotonNetwork.JoinLobby();
    }

    //public override void OnJoinedLobby()
    //{
    //    base.OnJoinedLobby();
    //    Debug.Log("Lobby Joined");
    //    playButton.SetActive(true);
    //}

}
