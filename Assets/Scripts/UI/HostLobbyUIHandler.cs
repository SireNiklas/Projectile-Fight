using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HostLobbyUIHandler : MonoBehaviour
{
    GameObject UIObjectToHide, UIObjecToShow;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startGameBtn = root.Q<Button>("startgame");
        Button backBtn = root.Q<Button>("back");


        startGameBtn.clicked += () => Debug.Log("START GAME!!");
        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[3], UIObjecToShow = UIManager.Instance.UIObjects[2]);

        Button test = new Button();

        test.text = "Test";
    }
}
