using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : NetworkBehaviour
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
        //SceneManager.LoadScene("Game_Scene");
        HideUI();
        NetworkManager.Singleton.StartHost();
    }

    private void OnClickClientBtn()
    {
        //SceneManager.LoadScene("Game_Scene");
        HideUI();
        NetworkManager.Singleton.StartClient();
    }

    private void HideUI()
    {

        if (!IsClient && !IsOwner) return;
        this.gameObject.SetActive(false);

    }
}
