using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotate : MonoBehaviour
{
    [SerializeField]
    private int intialRotation = -155;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        Quaternion targetRotation = Quaternion.Euler(rectTransform.eulerAngles.x, intialRotation, rectTransform.eulerAngles.z);

        rectTransform.localRotation = targetRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
