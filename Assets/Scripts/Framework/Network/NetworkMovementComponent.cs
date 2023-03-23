using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class NetworkMovementComponent : NetworkBehaviour
{
	//TODO SERVER TICK RATE
	//private NetworkTime serverTick = NetworkManager.Singleton.ServerTime;
	//private int TestTick;
	
#region Player, Camera, Jump, Grounded

	#region Player 
	// Player References
	[Header("Player References")]
	[SerializeField] private CharacterController _controller;
	[SerializeField] private FirstPersonInputHandler _firstPersonInputHandler;

	// Player Settings
    [SerializeField] private float _speed;
    [Header("Player Parameters")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float MoveSpeed = 4.0f;
    [Tooltip("Walk speed of the character in m/s")]
    [SerializeField] private float WalkSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float SprintSpeed = 6.0f;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float SpeedChangeRate = 10.0f;
    
    // Player Variables
    private float _verticalVelocity;
    #endregion

    #region Camera
    // Camera References
    [Header("Camera References")]
    private GameObject _mainCamera;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject CinemachineCameraTarget;
    
    // Camera Settings
    [Header("Camera Parameters")]
    [Tooltip("Rotation speed of the character")]
    [Range(10, 30)]
    [SerializeField] private float RotationSpeed = 1.0f;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -90.0f;
    
    // Camera Variables
    private float _rotationVelocity;
    private float _cinemachineTargetPitch;
	#endregion
    
	#region Jump
	// Jump Settings
    [Header("Jump Parameters")]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float Gravity = -15.0f;
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField]private float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float FallTimeout = 0.15f;
    [Tooltip("Max falling speed")]
    [SerializeField] private float _terminalVelocity = 53.0f;
    // Jump Variables
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
	#endregion

	#region Grounded 
    // Grounded Settings
    [Header("Grounded Parameters")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    [SerializeField] private float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    #endregion
#endregion

    // Tick Settings
    private int _tick = 0;
    private float _tickRate = 1f / 60f;
    private float _tickRateDeltaTime = 0f;
    private int _lastProcessedTick = -0;

    // Memory Settings? Tick Memory?
    private const int BUFFER_SIZE = 1024;
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    // Local and Server Position Veriables
    public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
    public TransformState _previousTransformState;
    
    //TODO Gizmo Settings | Temp
    [Header("Gizmo Parameters")]
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Color _color;
    
    private void OnEnable()
    {
	    ServerTransformState.OnValueChanged += OnServerStateChanged;
    }

    private void Awake()
    {
	    _controller = GetComponent<CharacterController>();
	    _firstPersonInputHandler = GetComponent<FirstPersonInputHandler>();
        
	    // get a reference to our main camera
	    if (_mainCamera == null)
	    {
		    _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	    }
	    
	    if (_cinemachineVirtualCamera == null)
	    {
		    _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
	    }
    }

    private void Start()
    {
	    _jumpTimeoutDelta = JumpTimeout;
	    _fallTimeoutDelta = FallTimeout;
	    
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //TODO SERVER TICK
        //NetworkManager.NetworkTickSystem.Tick += Tick;
        
        if (IsClient && IsOwner)
        {
	        _cinemachineVirtualCamera.Follow = CinemachineCameraTarget.transform;
        }
    }

    //TODO SERVER TICK
    // private void Tick()
    // {
	   //  Debug.Log($"Tick: {NetworkManager.ServerTime.Tick}");
    //
	   //  var TestTick = NetworkManager.Singleton.NetworkTickSystem.ServerTime.Tick;
    // }

    private void OnServerStateChanged(TransformState previousState, TransformState serverState)
    {
	    if (!IsLocalPlayer) return;

	    if (_previousTransformState == null)
	    {
		    _previousTransformState = serverState;
	    }

	    TransformState calculatedState = _transformStates.First(localState => localState.Tick == serverState.Tick);

	    if (calculatedState.Position != serverState.Position)
	    {
		    Debug.Log("CORRECTING CLIENT POS!");
		    
		    //Teleport the player to server pos.
		    TeleportPlayer(serverState);

		    //Replay the inputs that happened after. | Change to for loop if needed. (Not optimised as is.
		    IEnumerable<InputState> inputs = _inputStates.Where(input => input.Tick > serverState.Tick);
		    // Lazy way of sorting, algorithm would be better.
		    inputs = from input in inputs orderby input.Tick select input;

		    foreach (InputState inputState in inputs)
		    {
			    MovePlayer(inputState.movementInput);
			    RotatePlayer(inputState.lookInput);

			    TransformState newTranformState = new TransformState()
			    {
				    Tick = inputState.Tick,
				    Position = transform.position,
				    Rotation = transform.rotation,
				    HasStartedMoving = true

			    };

			    // Could be written better once I understand how to write it better.
			    for (int i = 0; i < _transformStates.Length; i++)
			    {
				    if (_transformStates[i].Tick == inputState.Tick)
				    {
					    _transformStates[i] = newTranformState;
					    break;
				    }
			    }
		    }
	    }
    }

    private void TeleportPlayer(TransformState state)
    {
	    _controller.enabled = false;
	    transform.position = state.Position;
	    transform.rotation = state.Rotation;
	    _controller.enabled = true;

	    for (int i = 0; i < _transformStates.Length; i++)
	    {
		    if (_transformStates[i].Tick == state.Tick)
		    {
			    _transformStates[i] = state;
			    break;
		    }
	    }
    }

    public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
    {
        _tickRateDeltaTime += Time.deltaTime;
        if (_tickRateDeltaTime > _tickRate)
        {
            int bufferIndex = _tick % BUFFER_SIZE;

            if (!IsServer)
            {
	            MovePlayerServerRpc(_tick, movementInput, lookInput);
	            GroundedCheck();
	            JumpAndGravity();
	            MovePlayer(movementInput);
                RotatePlayer(lookInput);
                SaveState(movementInput, lookInput, bufferIndex);
            }
            else
            {
	            GroundedCheck();
	            JumpAndGravity();
	            MovePlayer(movementInput);
                RotatePlayer(lookInput);

                TransformState state = new TransformState()
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                SaveState(movementInput, lookInput, bufferIndex);
                
                _previousTransformState = ServerTransformState.Value;
                ServerTransformState.Value = state;
            }
            Debug.Log($"Local Transform UPDATE = {transform.position} | Server Transform UPDATE = {ServerTransformState.Value.Position}");


            _tickRateDeltaTime -= _tickRate;
            _tick++;
        }
    }

    public void ProcessSimulatePlayerMovement()
    {
        _tickRateDeltaTime += Time.deltaTime;
        if (_tickRateDeltaTime > _tickRate)
        {
            if (ServerTransformState.Value.HasStartedMoving)
            {
                transform.position = ServerTransformState.Value.Position;
                transform.rotation = ServerTransformState.Value.Rotation;
            }

            _tickRateDeltaTime -= _tickRate;
            _tick++;
        }
    }
    
    private void SaveState(Vector2 movementInput, Vector2 lookInput, int bufferIndex)
    {
	    InputState inputState = new InputState()
	    {
		    Tick = _tick,
		    movementInput = movementInput,
		    lookInput = lookInput
	    };
            
	    TransformState transformState = new TransformState()
	    {
		    Tick = _tick,
		    Position = transform.position,
		    Rotation = transform.rotation,
		    HasStartedMoving = true
	    };

	    _inputStates[bufferIndex] = inputState;
	    _transformStates[bufferIndex] = transformState;    
    }
    
#region Movement Logic
    private void MovePlayer(Vector2 movementInput)
    {
	    // calculate movement direction
        Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
        moveDirection.y = -9.8f;

        if (_firstPersonInputHandler.sprint) _speed = 6f;
        else _speed = 4f;

        _controller.Move(new Vector3(moveDirection.x, _verticalVelocity, moveDirection.z) * _speed * _tickRate);
    }
#endregion
    
#region Camera Logic
    private void RotatePlayer(Vector2 lookInput)
    {
	    _cinemachineTargetPitch -= lookInput.y * RotationSpeed * _tickRate;
        _rotationVelocity = lookInput.x * RotationSpeed * _tickRate;
        
        // clamp our pitch rotation
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        
        // Update Cinemachine camera target pitch
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        
        // rotate the player left and right
        transform.Rotate(Vector3.up * _rotationVelocity);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
#endregion

    private void GroundedCheck()
    {
	    // set sphere position, with offset
	    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
	    Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    
    private void JumpAndGravity()
    {
	    if (Grounded)
    	{
	        // reset the fall timeout timer
    		_fallTimeoutDelta = FallTimeout;
     
    		// stop our velocity dropping infinitely when grounded
    		if (_verticalVelocity < 0.0f)
    		{
    			_verticalVelocity = -2f;
    		}
     
    		// Jump
    		if (_firstPersonInputHandler.jump && _jumpTimeoutDelta <= 0.0f)
    		{
	            Debug.Log($"BEFORE JUMP {_verticalVelocity}");
	            
	            // the square root of H * -2 * G = how much velocity needed to reach desired height
    			_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                Debug.Log($"AFTER JUMP {_verticalVelocity}");
    		}
     
    		// jump timeout
    		if (_jumpTimeoutDelta >= 0.0f)
    		{
    			_jumpTimeoutDelta -= _tickRate;
    		}
    	}
    	else
    	{
    		// reset the jump timeout timer
    		_jumpTimeoutDelta = JumpTimeout;
     
    		// fall timeout
    		if (_fallTimeoutDelta >= 0.0f)
    		{
    			_fallTimeoutDelta -= _tickRate;
    		}
     
    		// if we are not grounded, do not jump
    		//isJumping = false;
    	}
     
    	// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
    	if (_verticalVelocity < _terminalVelocity)
    	{
    		_verticalVelocity += Gravity * _tickRate;
    	}
    }

    [ServerRpc]
    private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput)
    {
	    if (_lastProcessedTick + 1 != tick)
        {
	        Debug.Log("I missed a tick");
	        Debug.Log($"Received Tick {tick}");
        }
        
	    _lastProcessedTick = tick;
	    MovePlayer(movementInput);
        RotatePlayer(lookInput);

        TransformState currentTransformState = new TransformState()
        {
            Tick = tick,
            Position = transform.position,
            Rotation = transform.rotation,
            HasStartedMoving = true
        };

        _previousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = currentTransformState;
    }
    
    private void OnDrawGizmos()
    {
	    if (ServerTransformState.Value != null)
	    {
		    Gizmos.color = _color;
		    Gizmos.DrawMesh(_meshFilter.mesh, ServerTransformState.Value.Position);
	    }
    }

    private void OnDrawGizmosSelected()
    {
	    Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
	    Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

	    if (Grounded) Gizmos.color = transparentGreen;
	    else Gizmos.color = transparentRed;

	    // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
	    Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}
