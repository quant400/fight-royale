using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : NetworkBehaviour
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
            RpcSpawnPowerUp(i,random, i);
        
            SpawnPowerUp(i, powerUpList[random], spawnList[i]);
        }
    }

    [ClientRpc]
    private void RpcSpawnPowerUp(int powerUpId, int powerUpIndex, int spawnIndex)
    {
        SpawnPowerUp(powerUpId, powerUpList[powerUpIndex], spawnList[spawnIndex]);
    }

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