using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PowerUpBehavior : NetworkBehaviour
{
    public PowerUp PowerUp;
    public bool isAvailable = true;

    void Start()
    {
        SpawnPowerUp();
    }

    public void SpawnPowerUp()
    {
        Debug.Log("SpawnPowerUp", gameObject);
        isAvailable = true;
        gameObject.SetActive(true);
    }
    
    [ClientRpc]
    public void RpcSpawnPowerUp()
    {
        SpawnPowerUp();
    }

    public void DestroyYourSelf()
    {
        isAvailable = false;
        gameObject.SetActive(false);
    }

    public PowerUp GetPowerUp()
    {
        if (!isAvailable) return null;
        DestroyYourSelf();
        return PowerUp;
    }

    [Command]
    public void CmdDestroyPowerUp()
    {
        
    }

}
