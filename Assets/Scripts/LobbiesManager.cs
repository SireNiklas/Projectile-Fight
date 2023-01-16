using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class LobbiesManager : MonoBehaviour
{
    public static LobbiesManager Instance { get; private set; }

    // DATA
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_MAP = "Map";

    // EVENTS
    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    public enum GameMode
    {
        SnowFight
    }

    // Variables
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    private Lobby joinedLobby;
    private string playerName;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    public async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    [Command]
    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, GameMode gameMode) {
        try {
            Player player = GetPlayer();

            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions {
                IsPrivate = isPrivate,
                Player = player,
                Data = new Dictionary<string, DataObject> {
                    {KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, "Snowball Fight")},
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "Snowglobe")}

                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

            //Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);

        } catch (LobbyServiceException e) {
            Debug.LogError(e.Message);
        }
    }

    [Command]
    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    [Command]
    public async void JoinLobby(Lobby lobby)
    {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
        {
            Player = player
        });

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    [Command]
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void UpdatePlayerName(string playerName)
    {
        this.playerName = playerName;

        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    [Command]
    private async void LeaveLobby()
    {
        try {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }

    }

    public async void UpdateLobbyGameMode(GameMode gameMode)
    {
        try
        {
            Debug.Log("UpdateLobbyGameMode " + gameMode);

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    //new QueryFilter(QueryFilter.FieldOptions.S1, "Snowball Fight", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }

            };

            
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
            }
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }

    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    //[Command]
    //private void PrintPlayers()
    //{
    //    PrintPlayers(joinedLobby);
    //}

    //private void PrintPlayers(Lobby lobby)
    //{
    //    Debug.Log("Players in lobby: " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);

    //    foreach (Player player in lobby.Players)
    //    {
    //        Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
    //    }
    //}

    //[Command]
    //private async void UpdateLobbyGameMode(string gameMode)
    //{
    //    try
    //    {
    //        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
    //        {
    //            Data = new Dictionary<string, DataObject> {
    //                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
    //            }
    //        });

    //        joinedLobby = hostLobby;

    //        PrintPlayers(hostLobby);
    //    } catch (LobbyServiceException e)
    //    {
    //        Debug.LogError(e.Message);
    //    }

    //}

    //[Command]
    //private async void UpdatePlayerName(string newPlayerName) { 
        
    //    try {
    //        playerName = newPlayerName;

    //        await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
    //        {
    //            Data = new Dictionary<string, PlayerDataObject>
    //            {
    //                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
    //            }
    //        });

    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.LogError(e.Message);
    //    }
    //}

    //[Command]
    //private async void MigrateLobbyHost(int playerIndex)
    //{
    //    try
    //    {
    //        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
    //        {
    //            HostId = joinedLobby.Players[playerIndex].Id
    //        });

    //        joinedLobby = hostLobby;

    //        PrintPlayers(hostLobby);
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.LogError(e.Message);
    //    }
    //}

    //// Not Nessasary | Auto delete after everyone leaves.
    //[Command]
    //private void DeleteLobby()
    //{
    //    try
    //    {
    //        LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
    //    }
    //    catch (LobbyServiceException e)
    //    {
    //        Debug.LogError(e.Message);
    //    }
    //}
}
