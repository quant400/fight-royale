using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridMovement : MonoBehaviour
{

    private Vector3 thisObjectInitialPosition, CharacterBaseInitialPosition;

    [SerializeField]
    private RectTransform thisObjectRectTransform, CharacterBaseRectTransform;

    private bool isSelectionScreen = false;

    private void Awake()
    {
        thisObjectInitialPosition = thisObjectRectTransform.localPosition;

        CharacterBaseInitialPosition = CharacterBaseRectTransform.localPosition;
    }

    private void OnEnable()
    {
        thisObjectRectTransform.localPosition = new Vector3(thisObjectInitialPosition.x, thisObjectInitialPosition.y, thisObjectInitialPosition.z - 2000);

        //CharacterBaseRectTransform.localPosition = new Vector3(CharacterBaseInitialPosition.x, CharacterBaseInitialPosition.y, CharacterBaseInitialPosition.z - 2000);

        thisObjectRectTransform.DOLocalMoveZ(thisObjectInitialPosition.z, 2f).SetEase(Ease.OutExpo);

        //CharacterBaseRectTransform.DOLocalMoveZ(CharacterBaseInitialPosition.z, 2f).SetEase(Ease.OutExpo);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveRightCharacter()
    {
        thisObjectRectTransform.localPosition = new Vector3(thisObjectRectTransform.localPosition.x + 2000, thisObjectInitialPosition.y, thisObjectInitialPosition.z);

        thisObjectRectTransform.DOLocalMoveX(thisObjectRectTransform.localPosition.x - 2000, 0.5f);
    }

    public void MoveLeftCharacter()
    {
        thisObjectRectTransform.localPosition = new Vector3(thisObjectRectTransform.localPosition.x - 2000, thisObjectInitialPosition.y, thisObjectInitialPosition.z);

        thisObjectRectTransform.DOLocalMoveX(thisObjectRectTransform.localPosition.x + 2000, 0.5f);
    }

    public void GoToSelectionScreen()
    {
        thisObjectRectTransform.DOLocalMoveX(thisObjectRectTransform.localPosition.x - 40, 0.5f);

        isSelectionScreen = true;
    }

    public void BackFromSelectionScreen()
    {
        if(isSelectionScreen)
        {
            thisObjectRectTransform.DOLocalMoveX(thisObjectRectTransform.localPosition.x + 40, 0.5f);

            isSelectionScreen = false;
        }
    }
}
