using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateGameUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;
    private TextField lobbyNameTxt;
    private Toggle lobbyVisibilityToggle;
    private string testString;
    private bool isPublic;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        lobbyNameTxt = root.Q<TextField>("lobbyNameTxt");
        lobbyVisibilityToggle = root.Q<Toggle>("lobbyVisibilityToggle");

        Button createLobbyBtn = root.Q<Button>("createlobby");
        Button backBtn = root.Q<Button>("back");
        
        createLobbyBtn.clicked += () =>
        {
            UIManager.Instance.lobbyName = lobbyNameTxt.text;
            UIManager.Instance.isPublic = lobbyVisibilityToggle.value;
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
