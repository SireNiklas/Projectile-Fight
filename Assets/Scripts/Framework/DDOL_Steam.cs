using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL_Steam : NetworkBehaviour
{
    public static DDOL_Steam Instance;



    private void Awake()
    {
        Instance= this;
        DontDestroyOnLoad(this);
    }
}
