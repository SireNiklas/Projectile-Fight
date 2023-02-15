using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIHandler : MonoBehaviour
{
    SteamLobbyHandler steamLobbyHandler;
    GameObject UIObjectToHide, UIObjecToShow;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button hostGameBtn = root.Q<Button>("hostGameBtn");
        //Button joinGameBtn = root.Q<Button>("joinGameBtn");
        Button optionsBtn = root.Q<Button>("optionsBtn");
        Button quitBtn = root.Q<Button>("quitBtn");

        hostGameBtn.clicked += () => { UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]); };
        //joinGameBtn.clicked += () => { UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]); };
        //optionsBtn.clicked += () => { UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]); };
        quitBtn.clicked += () => Application.Quit();

    }
}
