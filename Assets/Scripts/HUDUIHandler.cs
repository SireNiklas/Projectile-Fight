using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUIHandler : MonoBehaviour
{

    private PlayerStatsHandler _playerStatsHandler;
    public Image _playerHealthBarImage;
    private int _playerHealth;

    private void Start()
    {
    }

    private void Update()
    {
        _playerHealthBarImage.fillAmount -= 1;
    }
}
