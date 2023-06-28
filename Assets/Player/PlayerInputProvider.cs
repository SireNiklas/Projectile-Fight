using UnityEngine;
using Rewired;

public class PlayerInputProvider: MonoBehaviour, PRN.InputProvider<PlayerInput> {

	private PlayerInput input;
    public float deltaLookY = 0f;
    public float deltaLookX = 0f;
    public bool pendingJump = false;

    // TODOTODOTODO ADD REWIRED!
    // The Rewired player id of this character
    public int playerId = 0;
    public Player player; // The Rewired Player

    private void Update() {
		input.forward = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
		input.right = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        deltaLookY += Input.GetAxis("Mouse X");
        deltaLookX += Input.GetAxis("Mouse Y");
        pendingJump |= Input.GetKeyDown(KeyCode.Space);
	}

	// You need to implement this method
	public PlayerInput GetInput() {
        input.deltaLookY = deltaLookY;
        input.deltaLookX = deltaLookX;
        input.jump = pendingJump;
        deltaLookY = 0f;
        deltaLookX = 0f;
        pendingJump = false;
		return input;
	}

}