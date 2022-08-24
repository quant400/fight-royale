
namespace leaderboardModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections;

    using UnityEngine;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;




    [System.Serializable]
    public class allTimeLEaderBoard
    {

        public List<assetClass> allTimeLeadboeardClass;
    }
    [System.Serializable]
    public class assetClass
    {
        public string _id;
        public int id;
        public int dailyScore;
        public int allTimeScore;
        public int dailySessionPlayed;
        public int totalSessionPlayed;
        public DateTime updatedAt;
        public int __v;
        public string name;
    }
    [System.Serializable]

    public class userPostedData
    {
        public int id;
        public int score;
    }
    [System.Serializable]

    public class userGetDataModel
    {
        public int id;
    }

}



