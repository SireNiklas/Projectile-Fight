using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateGameUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button createLobbyBtn = root.Q<Button>("createlobby");
        Button backBrn = root.Q<Button>("back");


        //createLobbyBtn.clicked += () => /*UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[2], UIObjecToShow = UIManager.Instance.UIObjects[3])*/ UnityLobbyHandler.Instance.CreateGame();
        backBrn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[2], UIObjecToShow = UIManager.Instance.UIObjects[1]);
    }


}
