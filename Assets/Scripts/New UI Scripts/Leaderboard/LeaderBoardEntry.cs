using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardEntry : MonoBehaviour
{
    [SerializeField]
    TMP_Text rank, fighter, nftID, score;


    public void Set(string Rank,string Fighter,string ID,string Score)
    {
        rank.text = Rank;
        fighter.text = Fighter.ToUpper(); 
        nftID.text = ID;
        score.text = Score;

    }
        
}
