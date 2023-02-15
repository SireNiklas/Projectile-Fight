using QFSW.QC;
using Sir.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{

    public List<GameObject> UIObjects = new List<GameObject>();

    public string lobbyName = "Untitled Lobby";

    public void SwapUI(GameObject UIObjectToHide, GameObject UIObjectToShow)
    {
        UIObjectToHide.SetActive(false);
        UIObjectToShow.SetActive(true);
    }

    [Command]
    private void FindGame()
    {

    }
}
