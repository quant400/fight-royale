using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text textTitle;
    [SerializeField] private TMP_Text textTimer;

    public void SetupTimer(string title, int time)
    {
        textTitle.text = title;
        textTimer.text = string.Format("{0:00}",time);
        gameObject.SetActive(true);
    }

    public void UpdateTimer(int time)
    {
        textTimer.text = string.Format("{0:00}",time);
        if(time <= 0) StopTimer();
    }

    public void StopTimer()
    {
        gameObject.SetActive(false);
    }

    

}
