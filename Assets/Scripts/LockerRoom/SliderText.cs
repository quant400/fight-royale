using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private TMP_Text tMP_Text;

    public Slider slider;

    public Slider sliderGreen;

    // Start is called before the first frame update
    void Start()
    {
        tMP_Text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sliderGreen.value == 0)
        {
            tMP_Text.text = slider.value.ToString();
        }
        else
        {
            if(sliderGreen.value - slider.value > 0)
            {
                tMP_Text.text = slider.value.ToString() + " + " + (sliderGreen.value - slider.value).ToString();
            }
            else
            {
                tMP_Text.text = slider.value.ToString();
            }
        }

        
    }
}
