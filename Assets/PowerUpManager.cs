using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;

public class PowerUpManager : MonoBehaviour/*NetworkBehaviour*/
{
    public static PowerUpManager Instance;
    [SerializeField] private  List<GameObject> powerUpList;
    [SerializeField] private List<Transform> spawnList;

    public PowerUpBehavior GetPowerUpById(int powerUpId) 
    {
        Debug.Log("GetPowerUpById");

        var powerUps = FindObjectsOfType<PowerUpBehavior>();

        return powerUps.FirstOrDefault(aux => aux.id == powerUpId);
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SpawnPowerUps()
    {
        Debug.Log("SpawnPowerUps");
        for (int i = 0; i<spawnList.Count;i++)
        {
            int random = Random.Range(0, powerUpList.Count);

            // TODO Suleman, Uncomment later
            // Commented for Photon
            //RpcSpawnPowerUp(i, random, i);
            GameManager.Instance.player.RPC("RpcSpawnPowerUp", RpcTarget.AllBuffered, i, random, i);

            // Commented for Photon
            //SpawnPowerUp(i, powerUpList[random], spawnList[i]);
        }
    }

    public void SetupSpawnPowerUp(int powerUpId, int powerUpIndex, int spawnIndex)
    {
        SpawnPowerUp(powerUpId, powerUpList[powerUpIndex], spawnList[spawnIndex]);
    }

    // TODO Suleman, Uncomment later, done
    // Transferred PUN RPC to PlayerBehaviour.cs
    //[ClientRpc]
    //[PunRPC]
    //private void RpcSpawnPowerUp(int powerUpId, int powerUpIndex, int spawnIndex)
    //{
    //    SpawnPowerUp(powerUpId, powerUpList[powerUpIndex], spawnList[spawnIndex]);
    //}

    private void SpawnPowerUp(int powerUpId, GameObject powerUpGO, Transform spawn)
    {
        Debug.Log("SpawnPowerUp");
        DestroyChildren(spawn);
        var powerUp = Instantiate(powerUpGO, spawn.transform.position, spawn.transform.rotation, spawn);
        powerUp.GetComponent<PowerUpBehavior>().id = powerUpId;
        Debug.Log("FIM SpawnPowerUp");

    }

    public void DestroyChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(0));
        }
    }

}