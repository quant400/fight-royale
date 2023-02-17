using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoNFTCanvasScript : MonoBehaviour
{
   public void Buy()
    {
        Application.OpenURL("https://pancakeswap.finance/swap?outputCurrency=0x4f39c3319188a723003670c3f9b9e7ef991e52f3&inputCurrency=0xe9e7cea3dedca5984780bafc599bd69add087d56%0D%0A/");
    }

    public void Mint()
    {
        Application.OpenURL("https://app.cryptofightclub.io/mint");

    }

    public void ChangeWallet()
    {
        //gameplayView.instance.GetComponent<WebLogin>().OnLogin();
    }
}
