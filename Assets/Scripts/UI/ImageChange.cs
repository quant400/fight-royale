using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChange : MonoBehaviour
{
    Image objectImage;

    public Sprite gloveSprite;

    public Sprite pantsSprite;

    // Start is called before the first frame update
    void Start()
    {
        objectImage = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SkinnedMeshRendererBones.Instance.GetisShorts())
        {
            objectImage.sprite = pantsSprite;
        }
        else
        {
            objectImage.sprite = gloveSprite;
        }
    }


}
