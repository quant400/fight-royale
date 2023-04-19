using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceDisplayScript : MonoBehaviour
{
    public GameObject juiceText, CoinText, display;
    string juiceBal = "0";
    string coinBal = "0";

    public void ActivateJuiceDisplay()
    {
        display.SetActive(true);
    }
    public void DeactivateJuiceDisplay()
    {
        display.SetActive(false);
    }
    public void SetJuiceBal(string val)
    {
        juiceBal = val;
        UpdateJuiceBalance();
    }
    public void SetCoinBal(string val)
    {
        //val = "0.089898";

        string[] breakdown = val.Split('.');
        coinBal = breakdown[0] + "." + breakdown[1].Substring(0, 2);
        UpdateCoinBalance();
    }
    public void UpdateJuiceBalance()
    {
        if (juiceBal == "")
            juiceText.GetComponent<TMPro.TMP_Text>().text = "0";
        else
            juiceText.GetComponent<TMPro.TMP_Text>().text = juiceBal;
    }

    public void UpdateCoinBalance()
    {

        if (coinBal == "")
            CoinText.GetComponent<TMPro.TMP_Text>().text = "0";
        else
            CoinText.GetComponent<TMPro.TMP_Text>().text = coinBal;
    }
}
