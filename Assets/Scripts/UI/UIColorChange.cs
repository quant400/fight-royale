using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorChange : MonoBehaviour
{
    public GameObject glovesObject;

    public GameObject shortsObject;

    public GameObject platformObject;

    public Sprite glovesSprite;

    public Sprite shortsSprite;

    public Sprite platformSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor()
    {
        glovesObject.GetComponent<Image>().sprite = glovesSprite;

        shortsObject.GetComponent<Image>().sprite = shortsSprite;

        platformObject.GetComponent<Image>().sprite = platformSprite;
    }
}
