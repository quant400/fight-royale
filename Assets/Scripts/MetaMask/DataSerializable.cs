using System;
using System.Collections.Generic;


namespace CFC.Serializable
{

    [Serializable]
    public class Attribute2
    {
        public string trait_type;
        public string value;
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


