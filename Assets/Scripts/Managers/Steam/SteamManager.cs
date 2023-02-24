using UnityEngine;
using Steamworks;
using Steamworks.Data;
using System;
using QFSW.QC;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Sir.Core.Singletons;
using System.Linq;
using System.Collections.Generic;
using Mono.CSharp;

public class SteamManager : Singleton<SteamManager>
{
    private FacepunchTransport transport = null;
    public Lobby? lobby { get; private set; } = null;
    public Lobby currLobby { get; private set; }
    public SteamId currLobbySelected { get; private set; }

    GameObject UIObjectToHide, UIObjecToShow;

    public bool isHost = false;

    public List<Lobby> activeLobbies = new List<Lobby>();
    public Lobby[] lobbiesList;
    
    public async void JoinLobby(SteamId _lobbyId)
    {
        SteamManager.Instance.lobbiesList = await SteamMatchmaking.LobbyList.WithMaxResults(5).WithKeyValue("isTesting", "TRUE").RequestAsync();
        
        foreach (Lobby lobby in SteamManager.Instance.lobbiesList.ToList())
        {
            activeLobbies.Add(lobby);
            if (lobby.Id == _lobbyId)
            {
                await lobby.Join();

            }
        }
        UIManager.Instance.UIObjects[1].SetActive(false);
    }

    // public void QueryMemebers(SteamId steamId)
    // {
    //     Debug.Log(currLobby.member);
    // }
    
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

        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
    }

    private void OnApplicationQuit()
    {
        Disconnected();
    }

    #region SteamEvents
    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Debug.Log("Lobby was not created.");
            return;
        }

        switch (!UIManager.Instance.isPublic)
        {
            case true:
                _lobby.SetPublic();
                break;
            case false:
                _lobby.SetPrivate();
                break;
        }

        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        _lobby.SetData("isTesting", "TRUE");
        _lobby.SetData("lobbyName", UIManager.Instance.lobbyName);
        //urrLobby = _lobby.Id;
        Debug.Log("LOBBY ID " + _lobby.Id);
    }
    
    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        for (int i = 0; i < currLobby.MemberCount; i++)
        {
            foreach (var member in currLobby.Members)
            {
                // currLobby.SetMemberData("memberName", SteamClient.Name);
                Debug.Log("Member Joined: " + member.Name);
            }
        }
        
        if (isHost)
        {
            return;
        }
        for (int i = 0; i < currLobby.MemberCount; i++)
        {
            foreach (var member in currLobby.Members)
            {
                currLobby.SetMemberData("memberName", SteamClient.Name);
                Debug.Log("Member Leave: " + currLobby.GetMemberData(member, "memberName"));
            }
        }
        currLobby = _lobby;
        StartClient(_lobby.Owner.Id);
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
        
        for (int i = 0; i < currLobby.MemberCount; i++)
        {
            foreach (var member in currLobby.Members)
            {
                currLobby.SetMemberData("memberName", SteamClient.Name);
                Debug.Log(currLobby.GetMemberData(member, "memberName"));
            }
        }
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
            lobby = _lobby;
        }
    }
    #endregion

    #region Netcode for Gameobjects Host and Client start methods.
    [Command]
    public async void StartHost(int _maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();
        
        isHost = true;

        //UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[2], UIObjecToShow = UIManager.Instance.UIObjects[3]);

        lobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);

    }

    public void StartClient(SteamId _steamId)
    {
        // This must be called prior to client start.
        transport.targetSteamId = _steamId;
        
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectedCallback;
        
       //UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[2], UIObjecToShow = UIManager.Instance.UIObjects[4]);

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client has started");
        }
    }

    public void Disconnected()
    {
        lobby?.Leave();
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
        //GameManager.Instance.HostCreated();
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        UIManager.Instance.UIObjects[0].SetActive(false);
        NetworkManager.Singleton.StartClient();
    }

    private void Singleton_OnClientDisconnectedCallback(ulong _clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectedCallback;
        if (_clientId == 0)
        {
            Disconnected();
        }
    }
    #endregion

    //[Command]
    //public void CopyID()
    //{
    //    TextEditor textEditor = new TextEditor();
    //    textEditor.text = lobbyId;
    //    textEditor.SelectAll();
    //    textEditor.Copy();

    //}

}
