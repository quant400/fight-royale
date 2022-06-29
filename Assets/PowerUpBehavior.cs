using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PowerUpBehavior : NetworkBehaviour
{
    public PowerUp PowerUp;
    public bool isAvailable = true;
    
    public override void OnStartClient()
    { 
        gameObject.SetActive(true);
    }

    public void DestroYourSelf()
    {
        isAvailable = false;
        Destroy(gameObject);
    }

    public PowerUp GetPowerUp()
    {
        if (!isAvailable) return null;
        DestroYourSelf();
        return PowerUp;
    }

    [Command]
    public void CmdDestroyPowerUp()
    {
        
    }

}
