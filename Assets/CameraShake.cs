using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public void Shake(CinemachineImpulseSource source)
    {
        source.GenerateImpulse();
    }
}
