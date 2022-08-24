using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text textTitle;
    [SerializeField] private TMP_Text textTimer;
    [SerializeField] private Image fillImage;
    int initialTime;

    public void SetupTimer(string title, int time)
    {
        if(textTimer!=null)
            textTitle.text = title.ToUpper();
        textTimer.text = string.Format("{0:00}",time);
        textTimer.fontSize = textTitle.fontSize;
        if (fillImage != null)
        {
            fillImage.transform.parent.gameObject.SetActive(true);
            fillImage.fillAmount = 1f;
        }
        initialTime = time;
        gameObject.SetActive(true);
    }

    public void UpdateTimer(int time)
    {
        if (textTimer.fontSize != textTitle.fontSize)
            textTimer.fontSize = textTitle.fontSize;
        textTimer.text = string.Format("{0:00}",time);
        if (fillImage != null)
            fillImage.fillAmount = (float)time / (float)initialTime;
        if(time <= 0) StopTimer();
    }

    public void StopTimer()
    {
        gameObject.SetActive(false);
    }

    

}
