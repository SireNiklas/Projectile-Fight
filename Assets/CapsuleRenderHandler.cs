using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleRenderHandler : MonoBehaviour
{
    private NetworkMovementComponent _networkMovementComponent;

    # region  GIZMOS
    //TODO Gizmo Settings | Temp
    [Header("Gizmo Parameters")]
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Color _color;
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _networkMovementComponent = GetComponent<NetworkMovementComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos()
    {
        if (_networkMovementComponent.ServerTransformState.Value != null)
        {
            Gizmos.color = _color;
            Gizmos.DrawMesh(_meshFilter.mesh, _networkMovementComponent.ServerTransformState.Value.Position);
        }
    }
}
