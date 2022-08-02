using System;
using System.Collections.Generic;


namespace CFC.Serializable
{

    [Serializable]
    public class Attribute2
    {
        public string trait_type;
        public string value;
        public string _id;
    }

    [Serializable]
    public class AllUrl
    {
        public string name;
        public string link;
    }

    [Serializable]
    public class Account
    {
        public string name;
        public int id;
        public string description;
        public string image;
        public string external_url;
        public string animation_url;
        public bool isNFTForTesting;
        public List<Attribute2> attributes;
        public List<AllUrl> all_urls;

    }

    [Serializable]
    public class RootAccount
    {
        public List<Account> accounts;
    }
}


namespace CFC.Serializable.Leaderboard
{
    [Serializable]
    public class RootLeaderboard
    {
        public List<Scoreboard> scoreboards;
    }

    [Serializable]
    public class Scoreboard
    {
        public string _id;
        public int id;
        public int dailyScore;
        public int allTimeScore;
        public int dailySessionPlayed;
        public int totalSessionPlayed;
        public int kills;
        public DateTime updatedAt;
        public int __v;
        public string name;
    }

    public enum TypeScoreboard
    {
        AllTime = 0,
        Daily  = 1
    }

}

namespace CFC.Serializable.ListMultiplayerServers
{
    [Serializable]
    public class PostData
    {
        public string BuildId;
        public string Region;
    }

    [Serializable]
    public class Data
    {
        public List<MultiplayerServerSummary> MultiplayerServerSummaries;
        public int PageSize;
    }

    [Serializable]
    public class MultiplayerServerSummary
    {
        public string ServerId;
        public string SessionId;
        public string VmId;
        public string Region;
        public string State;
        public List<object> ConnectedPlayers;
        public DateTime LastStateTransitionTime;
        public string IPV4Address;
        public string FQDN;
        public List<Port> Ports;
        public bool CrashDumpUploaded;
    }

    [Serializable]
    public class Port
    {
        public string Name;
        public int Num;
        public string Protocol;
    }

    [Serializable]
    public class ListMultiplayerServersResponse
    {
        public int code;
        public string status;
        public Data data;
    }
}

namespace CFC.Serializable.GetEntityToken
{
    [Serializable]
    public class Data
    {
        public string EntityToken;
        public DateTime TokenExpiration;
        public Entity Entity;
    }
    [Serializable]
    public class Entity
    {
        public string Id;
        public string Type;
        public string TypeString;
        public bool IsTitle;
        public bool IsNamespace;
        public bool IsService;
        public bool IsMasterPlayer;
        public bool IsTitlePlayer;
    }
    [Serializable]
    public class GetEntityTokenResponse
    {
        public int code;
        public string status;
        public Data data;
    }

}

namespace CFC.Serializable.MultiplayerServers
{
    [Serializable]
    public class BuildSummary
    {
        public string BuildId;
        public string BuildName;
        public DateTime CreationTime;
        public object Metadata;
        public List<RegionConfiguration> RegionConfigurations;
    }

    [Serializable]
    public class CurrentServerStats
    {
        public int Active;
        public int StandingBy;
        public int Propping;
        public int Total;
    }

    [Serializable]
    public class Data
    {
        public List<BuildSummary> BuildSummaries;
        public int PageSize;
    }

    [Serializable]
    public class DynamicFloorMultiplierThreshold
    {
        public double TriggerThresholdPercentage;
        public double Multiplier;
    }

    [Serializable]
    public class DynamicStandbySettings
    {
        public bool IsEnabled;
        public int RampDownSeconds;
        public List<DynamicFloorMultiplierThreshold> DynamicFloorMultiplierThresholds;
    }

    [Serializable]
    public class RegionConfiguration
    {
        public string Region;
        public string Status;
        public int MaxServers;
        public int StandbyServers;
        public DynamicStandbySettings DynamicStandbySettings;
        public ScheduledStandbySettings ScheduledStandbySettings;
        public CurrentServerStats CurrentServerStats;
        public bool IsAssetReplicationComplete;
    }

    [Serializable]
    public class Root
    {
        public int code;
        public string status;
        public Data data;
    }

    [Serializable]
    public class ScheduledStandbySettings
    {
        public bool IsEnabled;
        public List<object> ScheduleList;
    }


}

namespace CFC.Serializable.Admin.TitleData
{
    [Serializable]
    public class RootKeys
    {
        public List<string> Keys;
    }

    [Serializable]
    public class Data
    {
        public string Key;
        public string Value;
    }

    [Serializable]
    public class DataKey
    {
        public Dictionary<string, string> dic;
    }
    
    [Serializable]
    public class RequestEndSession
    {
        public int id;
        public int score;
        public int kills;
    }
    
    [Serializable]
    public class RequestStartSession
    {
        public int id;
    }
  
}


