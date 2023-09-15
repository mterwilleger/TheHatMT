using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickupTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;

    [Header("Wall")]
    public Transform[] wallSpawnPoints;
    public string wallPrefabLocation;
    public GameObject[] myObjects;

    //instance
    public static GameManager instance; 

    void Awake ()
    {
        //instance
        instance = this;
    }

    void Start ()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame ()
    {
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer ()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer (int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer (GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }


    //Called when players exchange hats
    [PunRPC]
    public void GiveHat (int playerId, bool initialGive)
    {
        //Remove from current hatted player
        if(!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        //Give hat to new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }


    //Can player get hat at current time?
    public bool CanGetHat ()
    {
        if(Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

     public void OnCollisionEnter(Collision collision)
    {
      
        GameObject wallObj = PhotonNetwork.Instantiate(wallPrefabLocation, wallSpawnPoints[Random.Range(0, wallSpawnPoints.Length)].position, Quaternion.identity);
        SpawnObject();
    }
    
    public void SpawnObject()
    {
        //Spawns objects
        int randomIndex = Random.Range(0, myObjects.Length);
        Vector3 randomSpawnPosition = new Vector3(Random.Range(-15, 15), 10, Random.Range(-15, 15));

        Instantiate(myObjects[randomIndex], randomSpawnPosition, Quaternion.identity);
    }

    [PunRPC]
    void WinGame (int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        //Set the UI to show who's won
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu ()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
