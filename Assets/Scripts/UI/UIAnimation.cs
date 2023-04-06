using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimation : MonoBehaviour
{
    Vector3 startingPosition;

    RectTransform rectTransform;

    CanvasGroup canvasGroup;

    public bool upToDown = false;

    public bool downToUp = false;

    public bool rightToLeft = false;

    public bool leftToRight = false;

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        startingPosition = rectTransform.localPosition;

        //Debug.Log(startingPosition);

        //canvasGroup.alpha = 0;

        if (upToDown)
        {
            MoveUpToDown();
        }
        else if(downToUp)
        {

        }
        else if (downToUp)
        {

        }
        else if (rightToLeft)
        {

        }
        else if (leftToRight)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUpToDown()
    {
        rectTransform.localPosition = new Vector3(startingPosition.x, startingPosition.y + 100, startingPosition.z);

        //Debug.Log(startingPosition);

        rectTransform.DOLocalMoveY(startingPosition.y, 2f);

        //canvasGroup.DOFade(1, 2f);
    }
}
