using Sir.Core.Singletons;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsHandler : NetworkBehaviour
{

    public NetworkVariable<int> netHealth = new NetworkVariable<int>(100);

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateHealthServerRPC()
    {
        UpdateHealthClientRPC();
    }

    [ClientRpc]
    private void UpdateHealthClientRPC()
    {
        if (IsServer) DamageEnemy();
    }

    private void DamageEnemy()
    {
        netHealth.Value -= 1;
        Debug.Log(OwnerClientId + "; HEALTH VALUE: " + netHealth.Value);
    }
}
