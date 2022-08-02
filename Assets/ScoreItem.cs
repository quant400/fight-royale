using CFC.Serializable.Leaderboard;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Rank;
    [SerializeField] private TMP_Text text_Id;
    [SerializeField] private TMP_Text text_Name;
    [SerializeField] private TMP_Text text_Score;


    public void OnCreate(string rank,string id,string name,string score)
    {
        text_Rank.text = rank;
        text_Id.text = id;
        text_Name.text = name;
        text_Score.text = score;
    }

}
