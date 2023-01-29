using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnityLobbyHandler : MonoBehaviour
{
    public static UnityLobbyHandler Instance { get; private set; }

    private string lobbyCodeInput;

    [Command]
    private void HostGame()
    {
        Debug.Log("HOST GAME");
        UnityLobbyManager.Instance.CreateLobby();
        NetworkManager.Singleton.StartHost();
    }

    [Command]
    private void JoinGame()
    {
        UnityLobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput);
        NetworkManager.Singleton.StartClient();
    }

    [Command]
    public void CreateGame()
    {
        UnityLobbyManager.Instance.Authenticate("SirNiklas");
        UnityRelayManager.Instance.CreateRelay();
    }

}
