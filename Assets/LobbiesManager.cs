using QFSW.QC;
using Sir.Core.Singletons;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LobbiesManager : Singleton<LobbiesManager>
{

    // Lobbies List Variables
    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbiesListContent;

    public GameObject lobiesBtn, hostBtn;

    public List<GameObject> lobbiesList = new List<GameObject>();

    private SteamId _steamId;


    public void DestroyLobbies()
    {
        foreach(GameObject lobbyItem in lobbiesList)
        {
            Destroy(lobbyItem);
        }
        lobbiesList.Clear();
    }
}
