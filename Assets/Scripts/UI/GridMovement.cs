using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridMovement : MonoBehaviour
{

    private Vector3 initialPosition;

    [SerializeField]
    private RectTransform rectTransform;

    private void Awake()
    {
        initialPosition = rectTransform.localPosition;
    }

    private void OnEnable()
    {
        rectTransform.localPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z - 2000);

        rectTransform.DOLocalMoveZ(initialPosition.z, 2f).SetEase(Ease.OutExpo);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveRight()
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x + 1000, initialPosition.y, initialPosition.z);

        rectTransform.DOLocalMoveX(rectTransform.localPosition.x - 1000, 0.5f);
    }

    public void MoveLeft()
    {
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x - 1000, initialPosition.y, initialPosition.z);

        rectTransform.DOLocalMoveX(rectTransform.localPosition.x + 1000, 0.5f);
    }
}
