using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.SceneManager.LoadScene("Game_Scene", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
