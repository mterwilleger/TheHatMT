using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // instance
    public static NetworkManager instance;

    void Awake ()
    {
        if(instance != null && instance != this)
            gameObject.SetActive(false);
        else 
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start () 
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom (string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // changes the scene using Photon's system
    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
