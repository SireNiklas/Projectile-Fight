using System;
using QFSW.QC;
using Sir.Core.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsHandler : NetworkBehaviour
{
    //private HUDUIHandler _hUDUIHandler;

    public NetworkVariable<int> NetworkPlayerHealth = new NetworkVariable<int>();
    [SerializeField] private int playerHealth = 20;
    [SerializeField] private Slider _playerHealthBar;
    private int fillAmount;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkPlayerHealth.Value = 100;

    }

    private void OnEnable()
    {
        NetworkPlayerHealth.OnValueChanged += NetworkPlayerHealthChanged;
    }

    private void OnDisable()
    {
        NetworkPlayerHealth.OnValueChanged -= NetworkPlayerHealthChanged;
    }
    private void NetworkPlayerHealthChanged(int previousvalue, int newvalue)
    {
        _playerHealthBar.value = NetworkPlayerHealth.Value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Updates Clients Side!!!! DO NOT MESS THIS UP AGAIN!!!
        if (!IsServer) return;
        if (other.gameObject.CompareTag("Projectile") && OwnerClientId != other.GetComponent<NetworkObject>().OwnerClientId)
        {
            NetworkPlayerHealth.Value -= 10;
            Destroy(other.gameObject);
        }

        // if (other.gameObject.CompareTag("Static")) {
        //     Destroy(this);
        // }

    }

    // REFACTOR THIS!!! IT FEELS WRONG.
    private void Start()
    {
        //_playerHealthBar.maxValue = playerHealth;
    }

    // [ServerRpc(RequireOwnership = false)]
    // public void RequestUpdateHealthServerRPC()
    // {
    //     HealthUpdateClientRpc();
    // }
    //
    // [ClientRpc]
    // private void HealthUpdateClientRpc()
    // {
    //     if (!IsOwner) UpdateHealth();
    // }
    //
    // private void UpdateHealth()
    // {
    //     netHealth.Value = fillAmount;
    //     _playerHealthBar.value = fillAmount;
    //     if (playerHealth <= 0) this.NetworkObject.Despawn();
    // }
    //
    public void TakeDamage()
    {
        //networkplayerHealth.Value -= 10;
        //RequestUpdateHealthServerRPC();
    }
}
