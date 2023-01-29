using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomGameUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button createGameBtn = root.Q<Button>("creategame");
        Button backBtn = root.Q<Button>("back");

        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[1], UIObjecToShow = UIManager.Instance.UIObjects[0]);

        createGameBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[1], UIObjecToShow = UIManager.Instance.UIObjects[2]);
    }


}
