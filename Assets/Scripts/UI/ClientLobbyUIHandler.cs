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

        lobbyNameLbl.text = UIManager.Instance.lobbyName;
        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[4], UIObjecToShow = UIManager.Instance.UIObjects[1]);
    }


}
