using Cinemachine;
using Sir.Core.Singletons;
using Unity.Netcode;
using UnityEngine;
using Rewired;

public class FirstPersonInputHandler : NetworkBehaviour
{

	[SerializeField] private GameObject _hud;
	
	// The Rewired player id of this character
	public int playerId = 0;
	public Player player; // The Rewired Player

	[SerializeField] private NetworkMovementComponent _networkMovementComponent;

	Vector2 movementInput;
	Vector2 lookInput;

	public bool jump;
	public bool sprint;
	
	private void Awake()
	{
		// Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
		player = ReInput.players.GetPlayer(playerId);
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
    {
	    GetInput();
	    ProcessInput();
    }

    private void GetInput()
    {
	    movementInput.x = player.GetAxis("Move Horizontal");
	    movementInput.y = player.GetAxis("Move Vertical");
        
	    lookInput.x = player.GetAxis("Look Horizontal");
	    lookInput.y = player.GetAxis("Look Vertical");
	    
	    jump = player.GetButtonDown("Jump");
	    sprint = player.GetButtonDown("Sprint");
    }

    private void ProcessInput()
    {
	    if (IsClient & IsLocalPlayer)
	    { 
		    _networkMovementComponent.ProcessLocalPlayerMovement(movementInput, lookInput);
	    }
	    else
	    {
		    _networkMovementComponent.ProcessSimulatePlayerMovement();

	    }


    }
}