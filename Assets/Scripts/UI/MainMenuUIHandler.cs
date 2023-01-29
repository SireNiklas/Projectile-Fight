using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button quickJoinBtn = root.Q<Button>("quickjoin");
        Button customGameBtn = root.Q<Button>("customgame");
        Button quickBtn = root.Q<Button>("quit");


        quickJoinBtn.clicked += () => Debug.Log("QUICK JOIN!");
        customGameBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]);
        quickBtn.clicked += () => Application.Quit();
    }


}