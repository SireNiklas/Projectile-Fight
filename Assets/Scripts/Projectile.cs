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

        if (other.gameObject.CompareTag("Player") && other.gameObject != Parent) {
            PlayerStatsHandler.RequestUpdateHealthServerRPC();
        }

    }
}
