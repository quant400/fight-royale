using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Mirror;
using Mirror.Websocket;
using PlayFab.Helpers;

public class ClientStartUp : MonoBehaviour
{
	public Configuration configuration;
	public ServerStartUp serverStartUp;
	public Mirror.NetworkManager networkManager;
	//public TelepathyTransport telepathyTransport;
	//public ApathyTransport apathyTransport;
	public WebsocketTransport webTransport;


	public void OnLoginUserButtonClick()
	{
		if (configuration.buildType == BuildType.REMOTE_CLIENT)
		{
			if (configuration.buildId == "")
			{
				throw new Exception("A remote client build must have a buildId. Add it to the Configuration. Get this from your Multiplayer Game Manager in the PlayFab web console.");
			}
			else
			{
				LoginRemoteUser();
			}
		}
		else if (configuration.buildType == BuildType.LOCAL_CLIENT)
		{
			networkManager.StartClient();
		}
	}

	public void LoginRemoteUser()
	{
		Debug.Log("[ClientStartUp].LoginRemoteUser");
		
		//We need to login a user to get at PlayFab API's. 
		LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			CustomId = GUIDUtility.getUniqueID()
		};

		PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
	}

	private void OnLoginError(PlayFabError response)
	{
		Debug.Log(response.ToString());
	}

	private void OnPlayFabLoginSuccess(LoginResult response)
	{
		//Debug.Log(response.ToString());
		//Debug.Log(configuration.ipAddress == "");
		//Debug.Log(gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId==null);

		if (configuration.ipAddress == "" || gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId==null)
		{   //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
			//Debug.Log(1);
			RequestMultiplayerServer(); 
		}
		else
		{
			//Debug.Log(2);
			ConnectRemoteClient();
		}
	}

	private void RequestMultiplayerServer()
	{
		Debug.Log("[ClientStartUp].RequestMultiplayerServer");
		RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
		requestData.BuildId = configuration.buildId;
		requestData.SessionId = System.Guid.NewGuid().ToString();
		requestData.PreferredRegions = new List<AzureRegion>() { AzureRegion.EastUs };
		PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
	}

	private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
	{
		Debug.Log(response.ToString());
		ConnectRemoteClient(response);
	}

	private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
	{
		if (response == null)
		{ 
			networkManager.networkAddress = configuration.ipAddress;
			webTransport.port = configuration.port;
				
			
		}
		else
		{
			Debug.Log("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
			networkManager.networkAddress = response.IPV4Address;
			webTransport.port = (ushort)response.Ports[0].Num;
		}
		PlayerPrefs.SetString("ConnectInfo", configuration.buildId + "/" + gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId + "/" + "EastUs");
		networkManager.StartClient();
	}

	private void OnRequestMultiplayerServerError(PlayFabError error)
	{
		Debug.Log(error.ErrorDetails);
	}
}