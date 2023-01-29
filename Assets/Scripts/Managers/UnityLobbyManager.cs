using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UnityLobbyManager : MonoBehaviour
{
    public static UnityLobbyManager Instance { get; private set; }

    private Lobby lobby;
    private float lobbyHeartBeat;
    private Coroutine lobbyHeartBeatCoroutine;
    private string playerName;
    private bool isLobbyPrivate;

    // Start is called before the first frame update
    //async void Start()
    //{
    //    await UnityServices.InitializeAsync();

    //    AuthenticationService.Instance.SignedIn += () =>
    //    {
    //        Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
    //    };

    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();

    //    playerName = "SirNiklas" + UnityEngine.Random.Range(0, 99);
    //    Debug.Log(playerName);
    //}

    [Command]
    public async void Authenticate(string PlayerName)
    {
        this.playerName = PlayerName;
        InitializationOptions initializationOptions= new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "SirNiklas" + UnityEngine.Random.Range(0, 99);
        Debug.Log(playerName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LobbyHeartBeatCoroutine(string lobbyID, float waitTimeSeconds)
    {
        while (true)
        {
            Debug.Log("HeartBeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    //[Command]
    //public async void StartGame()
    //{
    //    if (islobbyHost)
    //    {
    //        try
    //        {
    //            Debug.Log("StartGame");

    //            string relayCode = await RelayManager.Instance.CreateRelay();

    //            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync()
    //        }
    //    }
    //}

    [Command]
    public async void CreateLobby()
    {
        string _lobbyName = "TestLobby1";
        int _lobbyMaxPlayers = 4;

        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {  "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                }
            };

            Lobby _lobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _lobbyMaxPlayers);

            lobby = _lobby;

            Debug.Log("Created ! " + _lobbyName + " " + _lobbyMaxPlayers + " " + lobby.LobbyCode);

            lobbyHeartBeatCoroutine = StartCoroutine(LobbyHeartBeatCoroutine(lobby.Id, 6f));

            //NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    private async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }

    }

    [Command]
    private async void JoinedLobbyById()
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("Joined lobby with code! " + lobbyCode);

            //NetworkManager.Singleton.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    /*
        try
        {

        } catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    */

    [Command]
    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }

        } catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }
}
