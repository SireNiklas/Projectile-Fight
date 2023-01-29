using QFSW.QC;
using Sir.Core.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsHandler : NetworkBehaviour
{
    //private HUDUIHandler _hUDUIHandler;

    public NetworkVariable<int> netHealth = new NetworkVariable<int>(100);
    [SerializeField] private int playerHealth = 20;
    [SerializeField] private Slider _playerHealthBar;
    private int fillAmount;


    // REFACTOR THIS!!! IT FEELS WRONG.
    private void Start()
    {
        _playerHealthBar.maxValue = playerHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateHealthServerRPC()
    {
        HealtUpdateClientRpc();
    }

    [ClientRpc]
    private void HealtUpdateClientRpc()
    {

        if (IsServer) UpdateHealth();


    }

    private void UpdateHealth()
    {
        netHealth.Value = fillAmount;
        if (playerHealth <= 0) this.NetworkObject.Despawn();
    }

    public void TakeDamage()
    {
        fillAmount = playerHealth -= 1;
    }

    public void LocalUpdateHealth()
    {

        _playerHealthBar.value = fillAmount;

    }
}
