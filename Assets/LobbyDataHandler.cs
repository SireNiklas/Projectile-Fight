using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class LobbyDataHandler : MonoBehaviour
{
    // Data
    public SteamId lobbyID;
    public string lobbyName;
    public TMP_Text lobbyNameText;

    public void SetLobbyData()
    {
        if (lobbyName == "") lobbyName = "Null";
        lobbyNameText.text = lobbyName;
    }
}
