using StarterAssets;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CombatController : NetworkBehaviour
{
    private FirstPersonController _firstPersonController;
    [SerializeField] private Projectile _projectile;
    //[SerializeField] private AudioClip _spawnClip;
    [SerializeField] private float _projectileSpeed = 700;
    [SerializeField] private float _cooldown = 0.5f;
    [SerializeField] private Transform _spawner;

    private float _lastFired = float.MinValue;
    private bool _fired;

    private void Start()
    {
        _firstPersonController = GetComponent<FirstPersonController>();
    }

private void Update()
    {
        if (!IsOwner) return;

        if (_firstPersonController._input.shoot && _lastFired + _cooldown < Time.time)
        {
            _firstPersonController._input.shoot = false;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
            {

                _lastFired = Time.time;
                var dir = raycastHit.point;

                // Send off the request to be executed on all clients
                RequestFireServerRpc(dir);

                // Fire locally immediately
                ExecuteShoot(dir);
                StartCoroutine(ToggleLagIndicator());
            }
            else
            {

                _lastFired = Time.time;
                var dir = Camera.main.transform.forward;

                // Send off the request to be executed on all clients
                RequestFireServerRpc(dir);

                // Fire locally immediately
                ExecuteShoot(dir);
                StartCoroutine(ToggleLagIndicator());
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

    private void ExecuteShoot(Vector3 dir)
    {
        Vector3 aimDir;

        if (dir != Camera.main.transform.forward) aimDir = (dir - _spawner.position).normalized;
        else aimDir = Camera.main.transform.forward;
        var projectile = Instantiate(_projectile, _spawner.position, Quaternion.LookRotation(aimDir, Vector3.up));
        projectile.Init(aimDir * _projectileSpeed);
        projectile.PlayerStatsHandler = this.GetComponent<PlayerStatsHandler>();
        projectile.Parent = this.gameObject;
        //AudioSource.PlayClipAtPoint(_spawnClip, transform.position);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (_fired) GUILayout.Label("FIRED LOCALLY");

        GUILayout.EndArea();
    }

    /// <summary>
    /// If you want to test lag locally, go into the "NetworkButtons" script and uncomment the artificial lag
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToggleLagIndicator()
    {
        _fired = true;
        yield return new WaitForSeconds(0.2f);
        _fired = false;
    }
}
