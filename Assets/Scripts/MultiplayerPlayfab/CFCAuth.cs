using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Authenticators;
using UnityEngine;

public class CFCAuth : NetworkAuthenticator
{
    private readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();

    [Header("Client Username")] public string playerName;
    public int playerId;
    public string walletNfts;
    
    #region Messages

    public class AuthRequestMessage : MessageBase
    {
        public string authUsername;
        public int nftId;
        public string nftWallet;
    }

    public class AuthResponseMessage : MessageBase
    {
        public byte code;
        public string message;
    }

    #endregion
    
    #region Server

    public override void OnServerAuthenticate(NetworkConnection conn)
    {
        
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    private void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
    {
        Debug.Log($"Authentication Request: {msg.authUsername}");
        Debug.Log($"Id: {msg.nftId}");
        Debug.Log($"Wallet: {string.Join(", ",msg.nftWallet)}");

        if (connectionsPendingDisconnect.Contains(conn)) return;

        if (//(!PlayerBehaviour.playerNames.Contains(msg.authUsername) || true) &&
            GameManager.Instance.match.currentState == MatchManager.MatchState.Lobby &&
            (msg.nftWallet.Equals("Demo") || GameManager.Instance.analytics.CheckWallet(msg.nftWallet)))
        {
            if (!msg.nftWallet.Equals("Demo"))
            {
                PlayerBehaviour.playerNames.Add(msg.authUsername);
            }

            conn.authenticationData = msg;

            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                code = 100,
                message = "Success"
            };

            conn.Send(authResponseMessage);

            OnServerAuthenticated?.Invoke(conn);
        }
        else
        {
            connectionsPendingDisconnect.Add(conn);

            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                code = 200,
                message = "Username already in use... try again"
            };

            conn.Send(authResponseMessage);

            conn.isAuthenticated = false;

            StartCoroutine(DelayedDisconnect(conn, 1f));
        }
    }

    IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        conn.Disconnect();

        yield return null;

        connectionsPendingDisconnect.Remove(conn);
    }

    #endregion
    
    #region Client

    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }
    
    public override void OnClientAuthenticate(NetworkConnection conn)
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage
        {
            authUsername = playerName,
            nftId = playerId,
            nftWallet = walletNfts
        };

        conn.Send(authRequestMessage);
    }
    
    public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
    {
        if (msg.code == 100)
        {
            Debug.Log($"Authentication Response: {msg.message}");
            
            OnClientAuthenticated?.Invoke(conn);
        }
        else
        {
            Debug.LogError($"Authentication Response: {msg.message}");

            conn.isAuthenticated = false;
            
            conn.Disconnect();
            
            //NetworkManager.singleton.StopHost();
        }
    }

    #endregion
    

    

}
