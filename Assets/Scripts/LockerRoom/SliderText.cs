using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private TMP_Text tMP_Text;

    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        tMP_Text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tMP_Text.text = slider.value.ToString();
    }
}
