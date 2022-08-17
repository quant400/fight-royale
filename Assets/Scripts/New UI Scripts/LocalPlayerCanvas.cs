using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerCanvas : MonoBehaviour
{
    [SerializeField]
    GameObject OtherCanvas;
    [SerializeField]
    Image localPlayerHealth;

    private void Start()
    {
        if(GetComponentInParent<PlayerBehaviour>().isLocalPlayer)
        {
            OtherCanvas.SetActive(false);
            GetComponentInParent<PlayerStatsController>().SetHealthBar(localPlayerHealth);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
