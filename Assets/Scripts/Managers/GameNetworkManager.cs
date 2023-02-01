using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System;
using QFSW.QC;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Sir.Core.Singletons;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    private FacepunchTransport transport = null;
    public Lobby? currentLobby { get; private set; } = null;

    public ulong hostId;

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamMatchmaking_OnGameLobbyJoinRequested;

    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamMatchmaking_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
    }

    private void OnApplicationQuit()
    {
        Disconnected();
    }

    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Debug.Log("Lobby was not created.");
            return;
        }

        _lobby.SetPublic(); // Can be private lobby.
        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        Debug.Log("lobby created FakeSteamName");

    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }
        Debug.Log("ENTERED LOBBY 1");
        StartClient(currentLobby.Value.Owner.Id);
        Debug.Log("ENTERED LOBBY 2");

    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
    {
        Debug.Log("Member joined");

    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _friend)
    {
        Debug.Log("Member Left");
    }

    // Friend sends a steam invite
    private void SteamMatchmaking_OnLobbyInvite(Friend _friend, Lobby _lobby)
    {
        Debug.Log($"Invite from {_friend.Name}");
    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _steamId)
    {
        Debug.Log("Lobby was created.");
    }

    // When game invite accept, or join on friend.
    private async void SteamMatchmaking_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _id)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if(joinedLobby != RoomEnter.Success)
        {
            Debug.Log("Failed to create lobby");
        } else
        {
            currentLobby = _lobby;
            //GameManager.Instance.ConnectedAsClient();
            Debug.Log("Joined Lobby");
        }
    }

    [Command]
    public async void StartHost(int _maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();
        //GameManager.Instance.myClientId = NetworkManager.Singleton.LocalClientId;
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);
    }

    public void StartClient(SteamId _steamId)
    {
        Debug.Log("STARTED CLIENT, ALSO BEFORE CLIENT DISCONNECT AND CONNECT START.");

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectedCallback;
        transport.targetSteamId = _steamId;

        Debug.Log("STARTED CLIENT, ALSO BEFORE CLIENT START.");

        NetworkManager.Singleton.StartClient();

        Debug.Log("STARTED CLIENT, ALSO AFTER CLIENT START.");

        //GameManager.Instance.myClientId = NetworkManager.Singleton.LocalClientId;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has started");
        }
    }

    public void Disconnected()
    {
        currentLobby?.Leave();
        if (NetworkManager.Singleton!= null)
        {
            return;
        }
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        } else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
        }
        NetworkManager.Singleton.Shutdown(true);
        //GameManager.Instance.Disconnected();
        Debug.Log("Disconnected");
    }

    private void Singleton_OnServerStarted()
    {
        Debug.Log("Host Started");
        //GameManager.Instance.HostCreated();
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log($"Client has connected : AnotherFakeSteamName");
    }

    private void Singleton_OnClientDisconnectedCallback(ulong _clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
        if (_clientId == 0)
        {
            Disconnected();
        }
    }


    //[Command]
    //public void CopyID()
    //{
    //    TextEditor textEditor = new TextEditor();
    //    textEditor.text = lobbyId;
    //    textEditor.SelectAll();
    //    textEditor.Copy();

    //}

}
