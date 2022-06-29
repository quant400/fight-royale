using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    public static Camera_Manager Instance;
    public CinemachineVirtualCamera railCam;
    public CinemachineVirtualCamera followCam;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
