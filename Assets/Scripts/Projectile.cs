using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PlayerStatsHandler PlayerStatsHandler;
    public GameObject Parent;
    //[SerializeField] private AudioClip _destroyClip;
    //[SerializeField] private GameObject _particles;

    private Vector3 _dir;

    private void Start()
    {
    }

    public void Init(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
        Invoke(nameof(DestroyBall), 3);
    }

    private void DestroyBall()
    {
        //AudioSource.PlayClipAtPoint(_destroyClip, transform.position);
        //Instantiate(_particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Updates Clients Side!!!! DO NOT MESS THIS UP AGIAN!!!
        if (other.gameObject.CompareTag("Player") && other.gameObject != Parent) {
            other.gameObject.GetComponent<PlayerStatsHandler>().RequestUpdateHealthServerRPC();
            other.gameObject.GetComponent<PlayerStatsHandler>().TakeDamage();
            other.gameObject.GetComponent<PlayerStatsHandler>().LocalUpdateHealth();
            Destroy(this);
        }

        if (other.gameObject.CompareTag("Static")) {
            Destroy(this);
        }

    }
}
