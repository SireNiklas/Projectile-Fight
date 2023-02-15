using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateGameUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;
    private TextField lobbyNameTxt;
    private string testString;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        TextField lobbyNameTxt = root.Q<TextField>("lobbyNameTxt");

        Button createLobbyBtn = root.Q<Button>("createlobby");
        Button backBtn = root.Q<Button>("back");
        
        createLobbyBtn.clicked += () =>
        {
            UIManager.Instance.lobbyName = lobbyNameTxt.text;
            SteamManager.Instance.StartHost(4);
            this.gameObject.SetActive(false); 
        };
        backBtn.clicked += () =>
        {
            UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[2],
                    UIObjecToShow = UIManager.Instance.UIObjects[1]);
        };
    }

}
