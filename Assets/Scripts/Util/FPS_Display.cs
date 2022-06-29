using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_Display : MonoBehaviour
{

    public static FPS_Display Instance;

    float timeA;
    public int fps;
    public int lastFPS;
    public GUIStyle textStyle;

    //private GameObject player;
    
    // Use this for initialization
    void Start () {

        timeA = Time.timeSinceLevelLoad;

        //Cria a instancia e não permite ser destruído, ou destrói caso já exista
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    void Update () {
    
        //if(player == null) player = GameObject.FindWithTag("Player");
        
        if(Time.timeSinceLevelLoad  - timeA <= 1)
        {
            fps++;
        }
        else
        {
            lastFPS = fps + 1;
            timeA = Time.timeSinceLevelLoad;
            fps = 0;
        }
    }
    void OnGUI()
    {
        //if(player == null)
            GUI.Label(new Rect( 10,10, 30,30),"FPS: "+lastFPS,textStyle);
        /*else
            GUI.Label(new Rect( 10,10, 30,30),$"FPS: {lastFPS} \nPosition: {player.transform.position}", textStyle);
    */
    }


}
