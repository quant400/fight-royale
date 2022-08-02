using CFC.Serializable.Leaderboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard_Manager : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private ScoreItem ScoreItem_Prefab;
    [SerializeField] private TypeScoreboard Type;

    public void OnCreate(List<Scoreboard> scoreboards)
    {
        var i = 1;
        ClearContent();

        foreach (var item in scoreboards)
        {
            if (i > GetLimite())
            {
                break;
            }

            if (Type == TypeScoreboard.AllTime)
                ScoreItem_Prefab.OnCreate(i.ToString(), item.id.ToString(), item.name.ToString(), item.allTimeScore.ToString());  
            else
                ScoreItem_Prefab.OnCreate(i.ToString(), item.id.ToString(), item.name.ToString(), item.dailyScore.ToString());
                
            Instantiate(ScoreItem_Prefab.gameObject, Content);
            i++;
        }

    }

    private int GetLimite()
    {
        switch (Type)
        {
            case TypeScoreboard.AllTime:
                return 21;
            case TypeScoreboard.Daily:
                return 10;
            default:
                return 1;
        }
    }


    private void ClearContent()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
    }

}
