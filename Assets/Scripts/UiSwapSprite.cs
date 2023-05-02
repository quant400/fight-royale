using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UiSwapSprite : MonoBehaviour
{
    private Image objectImage;

    public Sprite onImage;

    public Sprite offImage;

    public AudioMixer audioMixer;

    public string audioSelection;

    // Start is called before the first frame update
    void Start()
    {
        objectImage = GetComponent<Image>();

        //Debug.Log(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        float volume;

        audioMixer.GetFloat(audioSelection, out volume);

        if (volume != -80)
        {
            if (objectImage.sprite == offImage)
            {
                objectImage.sprite = onImage;
            }
        }
        else
        {
            if (objectImage.sprite == onImage)
            {
                objectImage.sprite = offImage;
            }
        }
    }

    public void ChangeSprite()
    {
        if(objectImage.sprite == onImage)
        {
            objectImage.sprite = offImage;
        }
        else
        {
            objectImage.sprite = onImage;
        }
    }
}
