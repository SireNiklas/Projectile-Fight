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


        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[3], UIObjecToShow = UIManager.Instance.UIObjects[2]);
    }


}
