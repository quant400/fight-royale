﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Mirror;
using Mirror.Websocket;
using PlayFab.Helpers;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class ClientStartUp : MonoBehaviour
{
	public Configuration configuration;
	public ServerStartUp serverStartUp;
	public Mirror.NetworkManager networkManager;
	//public TelepathyTransport telepathyTransport;
	//public ApathyTransport apathyTransport;
	public WebsocketTransport webTransport;
	[SerializeField] private GameObject player;


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

		//networkManager.StartClient();
		//Debug.Log("Server ip: " + PhotonNetwork.PhotonServerSettings.AppSettings.Server);

		//PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
		StartCoroutine(SpawnPlayer());

		//We need to login a user to get at PlayFab API's. 
		//LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
		//{
		//	TitleId = PlayFabSettings.TitleId,
		//	CreateAccount = true,
		//	CustomId = GUIDUtility.getUniqueID()
		//};

		//PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
	}

    private IEnumerator SpawnPlayer()
    {
        Debug.Log("ClientStartUp - SpawnPlayer()");

		//if (GameManager.Instance == null) Debug.Log("Game Manager instance null");
		//GameObject GMObj = GameManager.Instance.gameObject;
		//ActivateObjects(GMObj);

		Debug.Log("ClientStartUp - Activate objects called");

        yield return new WaitForSeconds(4);
        GameObject playerObject = PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
		Debug.Log("Player Object: " + playerObject.name);
		Camera_Manager.Instance.railCam.gameObject.SetActive(false);
        GameManager.Instance.OnClientConnect(playerObject.GetPhotonView());

        Debug.Log("ClientStartUp - SpawnPlayer OnClientConnect called");

    }

	private void ActivateObjects(GameObject gm)
	{
		gm.SetActive(true);

		foreach (Transform child in gm.transform)
		{
			ActivateObjects(child.gameObject);
		}
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
		requestData.PreferredRegions = new List<AzureRegion>() { AzureRegion.NorthEurope };
		if(gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId == null)
			gameplayView.instance.TempSessionID = requestData.SessionId;
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
			//Debug.Log("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
			networkManager.networkAddress = response.IPV4Address;
			webTransport.port = (ushort)response.Ports[0].Num;
		}
		if(gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId == null)
			PlayerPrefs.SetString("ConnectInfo", configuration.buildId + "/" + gameplayView.instance.TempSessionID+ "/" + "NorthEurope");
		else
			PlayerPrefs.SetString("ConnectInfo", configuration.buildId + "/" + gameplayView.instance.apiPlayfab.CurrentMultiplayerServerSummary.SessionId + "/" + "NorthEurope");
		networkManager.StartClient();
	}

	private void OnRequestMultiplayerServerError(PlayFabError error)
	{
		Debug.Log(error.ErrorDetails);
	}
}