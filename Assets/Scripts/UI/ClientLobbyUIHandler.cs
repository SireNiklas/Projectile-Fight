using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientLobbyUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;
    
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button backBtn = root.Q<Button>("back");
        Label lobbyNameLbl = root.Q<Label>("lobbyNameLbl");

        Debug.Log(SteamManager.Instance.currLobby.GetData("lobbyName"));
        
        lobbyNameLbl.text = SteamManager.Instance.currLobby.GetData("lobbyName");
        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[4], UIObjecToShow = UIManager.Instance.UIObjects[1]);
    }


}
