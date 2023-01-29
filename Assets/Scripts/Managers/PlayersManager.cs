using Sir.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : Singleton<PlayersManager>
{

    public ulong _playerID;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {

        };
    }
}
