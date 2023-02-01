using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    GameObject UIObjectToHide, UIObjecToShow;

    public void HostCreated()
    {
        UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]);
        GameManager.Instance.isHost = true;
        GameManager.Instance.connected= true;
    }

    public void ClientConnected()
    {
        UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[0], UIObjecToShow = UIManager.Instance.UIObjects[1]);
        GameManager.Instance.isHost = false;
        GameManager.Instance.connected = true;
    }

    public void Disconnected()
    {
        UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[1], UIObjecToShow = UIManager.Instance.UIObjects[0]);
        GameManager.Instance.isHost = false;
        GameManager.Instance.connected = false;
    }
}
