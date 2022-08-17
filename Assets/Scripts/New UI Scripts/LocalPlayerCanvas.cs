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
            Debug.Log(2);
            OtherCanvas.SetActive(false);
            GetComponentInParent<PlayerStatsController>().SetHealthBar(localPlayerHealth);
        }
        else
        {
            Debug.Log(4);
            gameObject.SetActive(false);
        }
    }
}
