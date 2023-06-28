using System;
using UnityEngine;

public class PlayerProcessor: MonoBehaviour, PRN.Processor<PlayerInput, PlayerState> {

	private CharacterController controller;

	[SerializeField]
	private float movementSpeed = 8f;
    [SerializeField]
    private float lookSensitivity = 200f;
    [SerializeField]
	private float jumpHeight = 2.5f;

	[SerializeField]
	private float gravityForce = -9.81f;

	public Vector3 movement = Vector3.zero;
	public Vector3 gravity = Vector3.zero;

    // ADDED

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
    [SerializeField] private GameObject CinemachineCameraTarget;


    private void Awake() {
		controller = GetComponent<CharacterController>();
	}

    // You need to implement this method
    // Your player logic happens here

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public PlayerState Process(PlayerInput input, TimeSpan deltaTime) {

        _cinemachineTargetPitch -= input.deltaLookX * RotationSpeed;
        _rotationVelocity = input.deltaLookY * RotationSpeed;

        // clamp our pitch rotation
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Update Cinemachine camera target pitch
        CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

        // rotate the player left and right
        transform.Rotate(Vector3.up * _rotationVelocity * lookSensitivity * (float)deltaTime.TotalSeconds);

        //transform.Rotate(Vector3.up * input.deltaLookY * lookSensitivity * (float)deltaTime.TotalSeconds);
        movement = (transform.forward * input.forward + transform.right * input.right).normalized * movementSpeed * (float)deltaTime.TotalSeconds;
        if (controller.isGrounded) {
			gravity = Vector3.zero;
			if (input.jump) {
				gravity = Vector3.up * Mathf.Sqrt(jumpHeight * 2 * -gravityForce) * (float) deltaTime.TotalSeconds;
			}
        }
		if (gravity.y > 0) {
			gravity += Vector3.up * gravityForce * Mathf.Pow((float) deltaTime.TotalSeconds, 2);
		} else {
			gravity += Vector3.up * gravityForce * Mathf.Pow((float) deltaTime.TotalSeconds, 2) * 1.3f;
		}
		controller.Move(movement + gravity);
		return new PlayerState() {
			position = transform.position,
            rotation = transform.rotation,
            movement = movement,
			gravity = gravity
		};
	}

	// You need to implement this method
	// Called when an inconsistency occures
	public void Rewind(PlayerState state) {
		controller.enabled = false;
		transform.position = state.position;
        transform.rotation = state.rotation;
        movement = state.movement;
		gravity = state.gravity;
		controller.enabled = true;
	}

}