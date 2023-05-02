using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MysterBoxSpriteRotation : MonoBehaviour
{
    public float rotationSpeed = 2f;
    public float rotationAngle = 360f;
    public Ease easeType = Ease.Linear;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.DORotate(new Vector3(0f, 0f, rotationAngle), rotationSpeed, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(easeType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
