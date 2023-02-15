using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugMenuHandler: MonoBehaviour
{
    SteamLobbyHandler steamLobbyHandler;
    GameObject UIObjectToHide, UIObjecToShow;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button quickJoinBtn = root.Q<Button>("quickjoin");
        Button customGameBtn = root.Q<Button>("customgame");
        Button quickBtn = root.Q<Button>("quit");

        quickJoinBtn.clicked += () => { SteamManager.Instance.StartHost(6); this.gameObject.SetActive(false); };
        customGameBtn.clicked += () => { 
            UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]); 
            /*steamLobbyHandler.CustomGame(); */
        };
        quickBtn.clicked += () => Application.Quit();
    }


}
