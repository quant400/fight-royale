using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectedWearable : MonoBehaviour
{
    [SerializeField]
    private GameObject[] wearableButtons;

    private Button[] objectButtons;

    private RectTransform[] rectTransforms;

    private CanvasGroup[] canvasGroups;

    private bool isWearableSelectionScreen;

    [SerializeField]
    private GameObject playerBody;
    private RectTransform playerBodyRectTransform;

    [SerializeField]
    private GameObject baseObject;
    private RectTransform baseObjectRectTransform;

    [SerializeField]
    private GameObject gridObject;
    private RectTransform gridObjectRectTransform;
    private CanvasGroup gridObjectCanvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        Intialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToWearableSelectionScreen()
    {
        if (!isWearableSelectionScreen)
        {
            //DiableInteractable();

            isWearableSelectionScreen = true;

            GoToSelectionScreenAnimation();
        }
    }

    public void WearableSelected()
    {
        if(isWearableSelectionScreen)
        {
            //EnableInteractable();

            isWearableSelectionScreen = false;

            WearableSelectedAnimation();
        }
    }

    private void Intialize()
    {
        isWearableSelectionScreen = false;

        playerBodyRectTransform = playerBody.GetComponent<RectTransform>();

        baseObjectRectTransform = baseObject.GetComponent<RectTransform>();

        gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

        gridObjectCanvasGroup = gridObject.GetComponent<CanvasGroup>();

        objectButtons = new Button[wearableButtons.Length];

        rectTransforms = new RectTransform[wearableButtons.Length];

        canvasGroups = new CanvasGroup[wearableButtons.Length];

        for (int i = 0; i < wearableButtons.Length; i++)
        {
            objectButtons[i] = wearableButtons[i].GetComponent<Button>();

            rectTransforms[i] = wearableButtons[i].GetComponent<RectTransform>();

            canvasGroups[i] = wearableButtons[i].GetComponent<CanvasGroup>();
        }
    }

    private void EnableInteractable()
    {
        foreach (Button objectButton in objectButtons)
        {
            objectButton.interactable = true;
        }
    }

    private void DiableInteractable()
    {
        foreach (Button objectButton in objectButtons)
        {
            objectButton.interactable = false;
        }
    }

    private void GoToSelectionScreenAnimation()
    {
        for (int i = 0; i < wearableButtons.Length; i++)
        {
            canvasGroups[i].DOFade(0.0f, 0.5f);

            rectTransforms[i].DOScale(new Vector3(0, 0, 0), 1);

            //playerBodyRectTransform.DOLocalMoveX(50, 0.5f);

            baseObjectRectTransform.DOLocalMoveX(-400, 0.5f);

            gridObjectCanvasGroup.DOFade(1.0f, 0.5f);

            gridObjectRectTransform.DOScale(new Vector3(1, 1, 1), 1);
        }
    }

    private void WearableSelectedAnimation()
    {
        for (int i = 0; i < wearableButtons.Length; i++)
        {
            canvasGroups[i].DOFade(1.0f, 0.5f);

            rectTransforms[i].DOScale(new Vector3(1, 1, 1), 1);

            //playerBodyRectTransform.DOLocalMoveX(0, 0.5f);

            baseObjectRectTransform.DOLocalMoveX(0, 0.5f);

            gridObjectCanvasGroup.DOFade(0.0f, 0.5f);

            gridObjectRectTransform.DOScale(new Vector3(0, 0, 0), 1);
        }
    }
}
