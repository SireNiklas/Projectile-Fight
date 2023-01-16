using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUIHandler : NetworkBehaviour
{
    [SerializeField] private GameObject DebugUIParent;

    private void OnEnable()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;

        Button serverBtn = rootElement.Q<Button>("serverBtn");
        Button hostBtn = rootElement.Q<Button>("hostBtn");
        Button clientBtn = rootElement.Q<Button>("clientBtn");

        serverBtn.clicked += () => StartServer();
        hostBtn.clicked += () => StartHost();
        clientBtn.clicked += () => StartClient();
    }

    private void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        DebugUIParent.SetActive(false);
    }
    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DebugUIParent.SetActive(false);
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DebugUIParent.SetActive(false);
    }
}
