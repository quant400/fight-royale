using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameplayView : MonoBehaviour
{
    public static gameplayView instance;

    public bool usingFreemint = false;

    public bool usingMeta = false;

    public (string, string) logedPlayer;

    public characterSelectionView csv;
    public CustomWebLogin cWL;
    
    public GameObject buttonsToEnableAftrLogin, buttonsToDisableAftrLogin;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
        
        DontDestroyOnLoad(this);
    }

    public string GetLoggedPlayerString()
    {
        if (usingMeta)
            return PlayerPrefs.GetString("Account");
        else
            return logedPlayer.Item1 + "$$$" + logedPlayer.Item2;
    }
}
