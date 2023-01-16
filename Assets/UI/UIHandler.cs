using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyBrowserUI;
    [SerializeField] private GameObject CreateLobbyUI;
    bool isCloseWindowClicked = false;

    private void OnEnable()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;

        Button lobbyBtn = rootElement.Q<Button>("lobbyBtn");
        Button optionsBtn = rootElement.Q<Button>("optionsBtn");
        Button quitBtn = rootElement.Q<Button>("quitBtn");

        Button createLobbyBtn = rootElement.Q<Button>("createLobbyBtn");
        Button RefreshLobbiesBtn = rootElement.Q<Button>("RefreshLobbiesBtn");
        Button closeWindowBtn = rootElement.Q<Button>("closeWindowBtn");

        lobbyBtn.clicked += () => ShowHideLobbyBrowser();

        createLobbyBtn.clicked += () => ShowHideCreateLobbyWindow();
        lobbyBtn.clicked += () =>
        closeWindowBtn.clicked += () => isCloseWindowClicked = true;

        quitBtn.clicked += () => Application.Quit();
    }

    private void ShowHideLobbyBrowser()
    {
        if (lobbyBrowserUI.activeSelf == true)
        {
            lobbyBrowserUI.SetActive(false);
        } else 
        {
            lobbyBrowserUI.SetActive(true);
        }
    }

    private void ShowHideCreateLobbyWindow()
    {

        if (lobbyBrowserUI.activeSelf == true && CreateLobbyUI.activeSelf == false)
        {
            CreateLobbyUI.SetActive(true);
        }

        if (lobbyBrowserUI.activeSelf == false || isCloseWindowClicked)
        {
            isCloseWindowClicked = false;
            CreateLobbyUI.SetActive(false);
        }

    }


}
