using Sir.Core.Singletons;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsHandler : NetworkBehaviour
{

    public NetworkVariable<int> netHealth = new NetworkVariable<int>(100);

    private void DamageEnemy()
    {
        netHealth.Value -= 1;
        Debug.Log(OwnerClientId + "; HEALTH VALUE: " + netHealth.Value);
    }

    [ClientRpc]
    private void UpdateHealthClientRPC()
    {
        if (!IsOwner) DamageEnemy();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateHealthServerRPC()
    {
        UpdateHealthClientRPC();
    }
}
