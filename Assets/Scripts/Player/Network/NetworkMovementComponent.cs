using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkMovementComponent : NetworkBehaviour
{
    [SerializeField] private CharacterController _controller;

    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;

    [SerializeField] private Transform _camSocket;
    [SerializeField] private GameObject _vCam;

    private Transform _vCamTransform;

    private int _tick = 0;
    private float _tickRate = 1f / 60f;
    private float _ticketDeltaTime = 0f;

    private const int BUFFER_SIZE = 1024;
    private InputState[] _inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
    public TransformState _PreviousTransformState;

    private void OnEnable()
    {
        ServerTransformState.OnValueChanged += OnServerStateChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _vCamTransform = _vCam.transform;
    }

    private void OnServerStateChanged(TransformState previousvalue, TransformState newvalue)
    {
        _PreviousTransformState = previousvalue;
    }

    public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
    {
        _ticketDeltaTime += Time.deltaTime;
        if (_ticketDeltaTime > _tickRate)
        {
            int bufferIndex = _tick % BUFFER_SIZE;

            if (!IsServer)
            {
                MovePlayerServerRpc(_tick, movementInput, lookInput);
                MovePlayer(movementInput);
                RotatePlayer(lookInput);
            }
            else
            {
                MovePlayer(movementInput);
                RotatePlayer(lookInput);

                TransformState currentTransformSate = new TransformState()
                {
                    Tick = _tick,
                    Postion = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                _PreviousTransformState = ServerTransformState.Value;
                ServerTransformState.Value = currentTransformSate;
            }

            InputState currentLocalInputState = new InputState()
            {
                Tick = _tick,
                movementInput = movementInput,
                lookInput = lookInput
            };

            TransformState currentLocalTransformSate = new TransformState()
            {
                Tick = _tick,
                Postion = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _inputStates[bufferIndex] = currentLocalInputState;
            _transformStates[bufferIndex] = currentLocalTransformSate;

            _ticketDeltaTime -= _tickRate;
            _tick++;
        }
    }

    public void ProcessSimulatePlayerMovement()
    {
        _ticketDeltaTime += Time.deltaTime;
        if (_ticketDeltaTime > _tickRate)
        {
            if (ServerTransformState.Value.HasStartedMoving)
            {
                transform.position = ServerTransformState.Value.Postion;
                transform.rotation = ServerTransformState.Value.Rotation;
            }

            _ticketDeltaTime -= _tickRate;
            _tick++;
        }
    }
    
    private void MovePlayer(Vector2 movementInput)
    {
        Vector3 movement = movementInput.x * _vCamTransform.right + movementInput.y * _vCamTransform.forward;

        movement.y = 0;
        if (!_controller.isGrounded)
        {
            movement.y = -9.61f;
        }

        _controller.Move(movement * _speed * _tickRate);
    }

    private void RotatePlayer(Vector2 lookInput)
    {
        _vCamTransform.RotateAround(_vCamTransform.position, _vCamTransform.right, -lookInput.y * _rotationSpeed * _tickRate);
        _vCamTransform.RotateAround(transform.position, transform.up, lookInput.x * _rotationSpeed * _tickRate);
        
    }

    [ServerRpc]
    private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput)
    {
        // if (tick != _PreviousTransformState.Tick + 1)
        // {
        //     Debug.Log("PACKET LOSS!");
        // }
        
        MovePlayer(movementInput);
        RotatePlayer(lookInput);

        TransformState currentTransformState = new TransformState()
        {
            Tick = tick,
            Postion = transform.position,
            Rotation = transform.rotation,
            HasStartedMoving = true
        };

        _PreviousTransformState = ServerTransformState.Value;
        ServerTransformState.Value = currentTransformState;
    }
}
