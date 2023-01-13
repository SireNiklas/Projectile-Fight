using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollisionHandler : NetworkBehaviour
{

    private PlayerStatsHandler _playerStatsHandler;

    private void Start()
    {
        _playerStatsHandler= GetComponent<PlayerStatsHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            //_playerStatsHandler.TakeDamage();
        }
    }

}
