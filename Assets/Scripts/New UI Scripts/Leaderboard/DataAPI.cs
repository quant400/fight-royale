using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//incorrect name to be honest. as this is a namespace for models, not api calls. 
namespace DataApi
{
    [Serializable]
    public class LeaderboardUser
    {
        public string assetID;
        public string userName;
        public int sessionCounter;
        public int userScore;
        public int userRank;

        public LeaderboardUser() { }
        public LeaderboardUser(string _assetID,string name,int _sessionCounter,int _score)
        {
            this.assetID = _assetID;
            this.userName = name;
            this.sessionCounter = _sessionCounter;
            this.userScore = _score;

        }
        // public LeaderboardUser(string id, string name, int score, int rank)
        // {
        //     this.assetID = id;
        //     this.userName = name;
        //     this.userScore = 0;
        //     this.userRank = 0;
        // }
    }

    [Serializable]
    public class leaderboardObject
    {
        public List<LeaderboardUser> users;
    }
}
