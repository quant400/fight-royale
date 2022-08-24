using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardControllerRestApi : MonoBehaviour
{
    [SerializeField]
    GameObject leaderBoard;
    [SerializeField]
    GameObject leaderboardEntryPrefab;
    [SerializeField]
    Transform layoutGroup;

    restApiDataView restApiView;

    private void Awake()
    {
        restApiView = GetComponent<restApiDataView>();
    }
   
    public void ToggleLeaderBoard(bool b)
    {
        if (b==true)
        {
            //restApiView.DisplayDailyLeaderboardRestApi();
            restApiView.DisplayLeaderboardRestApi();
            leaderBoard.GetComponent<LeaderBoardScript>().Activate();
        }
        else
        {
            leaderBoard.GetComponent<LeaderBoardScript>().Deactivate();

        }
    }

    public async void UpDateLeaderBoardDaily()
    {
       
    }
   
    public async void UpDateLeaderBoardMonthly()
    {
       
    }
    public void UpDateLeaderBoardDailyRestApi(leaderboardModel.assetClass[] _leaderboardObject, string _leaderboardHeader)
    {
        Clean();

        var query = new Dictionary<string, object>();
        query.Add("NumResults", 15);
        Clean();
        int score;
        int rank = 1;
        foreach (leaderboardModel.assetClass _user in _leaderboardObject)
        {
            if (_leaderboardHeader == "Daily LEADERBOARD")
            {
                score = _user.dailyScore;
            }
            else
            {
                score = _user.allTimeScore;
            }
           
                var temp = Instantiate(leaderboardEntryPrefab, layoutGroup);
                temp.GetComponent<LeaderBoardEntry>().Set(rank.ToString(), _user.name, _user.id.ToString(), _user.dailyScore.ToString());
            
            rank++;
        }
    }
    public void UpDateLeaderBoardAllTimeRestApi(leaderboardModel.assetClass[] _leaderboardObject, string _leaderboardHeader)
    {
        Clean();

        var query = new Dictionary<string, object>();
        query.Add("NumResults", 15);
        Clean();
        int score;
        int rank = 1;
        foreach (leaderboardModel.assetClass _user in _leaderboardObject)
        {
            if (_leaderboardHeader == "Daily LEADERBOARD")
            {
                score = _user.dailyScore;
            }
            else
            {
                score = _user.allTimeScore;
            }
           
                var temp = Instantiate(leaderboardEntryPrefab, layoutGroup);
                temp.GetComponent<LeaderBoardEntry>().Set(rank.ToString(), _user.name, _user.id.ToString(), _user.allTimeScore.ToString());
            
            rank++;
        }
    }
    void Clean()
    {
        for (int i = layoutGroup.childCount - 1; i >= 0; i--)
        {
            Destroy(layoutGroup.GetChild(i).gameObject);
        }
    }
}
