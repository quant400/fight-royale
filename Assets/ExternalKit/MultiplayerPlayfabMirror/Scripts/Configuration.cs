using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : MonoBehaviour
{
    public BuildType buildType;
    public string buildId;
    public string ipAddress => Connection_Manager.Instance.ServerIp;
    public ushort port => Connection_Manager.Instance.ServerPort;
    public bool playFabDebugging = false;
}

public enum BuildType
{
    LOCAL_CLIENT,
    REMOTE_CLIENT,
    LOCAL_SERVER,
    REMOTE_SERVER
}