using StarterAssets;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatController : NetworkBehaviour
{
    private FirstPersonController _firstPersonController;
    [SerializeField] private Projectile _projectile;
    //[SerializeField] private AudioClip _spawnClip;
    [SerializeField] private float _projectileSpeed = 700;
    [SerializeField] private float _cooldown = 0.5f;
    [SerializeField] private Transform _spawner;
    private PlayerStatsHandler _playerStatsHandler;

    private float _lastFired = float.MinValue;
    private bool _fired;

    private Vector3 aimDir;
    private Vector3 aimAt;

    [SerializeField] private AudioClip[] _audioClips;

    private void Start()
    {
        _firstPersonController = GetComponent<FirstPersonController>();
    }

private void Update()
    {

        if (_firstPersonController._input.shoot && _lastFired + _cooldown < Time.time)
        {
            _firstPersonController._input.shoot = false;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            aimAt = Camera.main.transform.forward;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
            {

                _lastFired = Time.time;
                aimAt = raycastHit.point;
                aimDir = (aimAt - _spawner.position).normalized;
                 
                // Should update clients side!!!!
                //raycastHit.collider.gameObject.GetComponent<PlayerStatsHandler>().UpdateHealthClientRPC();

                // Send off the request to be executed on all clients
                RequestFireServerRpc(aimDir);

                // Fire locally immediately
                ExecuteShoot(aimDir);
            }
            else
            {

                // Send off the request to be executed on all clients
                RequestFireServerRpc(aimAt);

                // Fire locally immediately
                ExecuteShoot(aimAt);
            }
        }
    }

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 dir)
    {
        FireClientRpc(dir);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 dir)
    {
        if (!IsOwner) ExecuteShoot(dir);
    }

    private AudioClip RandomClip()
    {
        return _audioClips[Random.Range(0, _audioClips.Length)];
    }
    
    private void ExecuteShoot(Vector3 dir, ServerRpcParams serverRpcParams = default)
    {
        AudioSource.PlayClipAtPoint(RandomClip(), transform.position);
        
        var projectile = Instantiate(_projectile, _spawner.position, Quaternion.LookRotation(dir, Vector3.up));
        projectile.Init(dir * _projectileSpeed);
        projectile.PlayerStatsHandler = GetComponent<PlayerStatsHandler>();
        projectile.Parent = gameObject;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (_fired) GUILayout.Label("FIRED LOCALLY");

        GUILayout.EndArea();
    }
}
