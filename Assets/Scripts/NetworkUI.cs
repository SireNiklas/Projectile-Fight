using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button _serverBtn;
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;

    private void Awake()
    {
        _serverBtn.onClick.AddListener(() => OnClickServerBtn());
        _hostBtn.onClick.AddListener(() => OnClickHostBtn());
        _clientBtn.onClick.AddListener(() => OnClickClientBtn());
    }

    private void OnClickServerBtn()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void OnClickHostBtn()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnClickClientBtn()
    {
        NetworkManager.Singleton.StartClient();
    }
}
