using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{

    [SerializeField] private CursorLockMode currentLockMode;
    
    void Start()
    {
        Cursor.lockState = currentLockMode;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            ChangeMode();
    }

    private void ChangeMode()
    {
        if (currentLockMode == CursorLockMode.None)
            currentLockMode = CursorLockMode.Locked;
        else
            currentLockMode = CursorLockMode.None;

        Cursor.lockState = currentLockMode;
    }
}
