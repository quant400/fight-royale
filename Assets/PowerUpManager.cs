using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerUpManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> powerUpList;
    [SerializeField] private List<Transform> spawnList;

    public void SpawnPowerUps()
    {
        Debug.Log("SpawnPowerUps");
        for (int i = 0; i<spawnList.Count;i++)
        {
            int random = Random.Range(0, powerUpList.Count);
            RpcSpawnPowerUp(random, i);
        
            SpawnPowerUp(powerUpList[random], spawnList[i]);
        }
    }

    [ClientRpc]
    private void RpcSpawnPowerUp(int powerUpIndex, int spawnIndex)
    {
        SpawnPowerUp(powerUpList[powerUpIndex], spawnList[spawnIndex]);
    }

    private void SpawnPowerUp(GameObject powerUpGO, Transform spawn)
    {
        Debug.Log("SpawnPowerUp");
        DestroyChildren(spawn);
        Instantiate(powerUpGO, spawn.transform.position, spawn.transform.rotation, spawn);
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